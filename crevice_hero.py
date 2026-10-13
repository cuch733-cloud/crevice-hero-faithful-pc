import csv
import json
import math
import os
import random
import re
import sys
from pathlib import Path

import pygame

# Faithful PC reconstruction based on the game's decompiled gameplay scripts and
# embedded char/map/gem data. Original Android services/ads are intentionally absent.
pygame.init()
pygame.display.set_caption("Crevice Hero — PC Reconstruction")
W, H = 1280, 800
FPS = 60
screen = pygame.display.set_mode((W, H), pygame.RESIZABLE)
clock = pygame.time.Clock()
BASE = Path(getattr(sys, "_MEIPASS", Path(__file__).resolve().parent))
ASSET = BASE / "assets"
ATLAS = ASSET / "original_atlases"
REF = BASE / "reference" / "original_data"
SAVE = Path.home() / ".crevice_hero_pc_save.json"

WHITE = (246, 242, 220)
GOLD = (255, 218, 58)
CYAN = (85, 238, 244)
DARK = (8, 20, 28)
BG = (19, 47, 60)
MUTED = (151, 177, 184)
FONT = pygame.font.SysFont("arial", 24, bold=True)
SMALL = pygame.font.SysFont("arial", 18)
TINY = pygame.font.SysFont("arial", 14)
TITLE_FONT = pygame.font.SysFont("arial", 54, bold=True)

def asset_path(n):
    return ATLAS / ("sharedassets1_assets_Texture2D_{}_Texture2D_{}.png".format(n, n))

def load_image(path, alpha=True):
    try:
        im = pygame.image.load(str(path))
        return im.convert_alpha() if alpha else im.convert()
    except Exception:
        return None

def load_atlas(n):
    return load_image(asset_path(n))

atlases = {n: load_atlas(n) for n in (15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28)}
rock_texture = atlases[15]
ORIGINAL_TEXTURES = ASSET / "original_textures"
background_textures = {
    theme: [load_image(ORIGINAL_TEXTURES / "bg_{}_back_{:02d}.png".format(theme, order))
            for order in range(3)]
    for theme in range(4)
}
stage_textures = {theme: load_image(ORIGINAL_TEXTURES / "bg_{}_stage.png".format(theme))
                  for theme in range(5)}
stage_floor_textures = {theme: load_image(ORIGINAL_TEXTURES / "bg_{}_stage_floor.png".format(theme))
                        for theme in range(5)}
stage_ceiling_textures = {theme: load_image(ORIGINAL_TEXTURES / "bg_{}_stage_ceiling.png".format(theme))
                          for theme in range(5)}
stage_tiles = {theme: pygame.transform.scale(im, (256, 256))
               for theme, im in stage_textures.items() if im is not None}
scaled_bg_cache = {}
title_logo = load_image(ASSET / "title_logo.png")
if title_logo:
    title_logo = pygame.transform.smoothscale(title_logo, (390, 191))

def crop_sprite(sheet, rect):
    if sheet is None:
        return None
    x, y, w, h = rect
    x = max(0, min(x, sheet.get_width() - 1))
    y = max(0, min(y, sheet.get_height() - 1))
    w = max(1, min(w, sheet.get_width() - x))
    h = max(1, min(h, sheet.get_height() - y))
    frame = sheet.subsurface((x, y, w, h)).copy()
    if frame.get_bounding_rect(min_alpha=8).width == 0:
        return None
    return frame

# The original build contains three character animation atlas families in the
# recovered sharedassets. We retain full-body cells only, then animate with the
# matching move/jump-like poses rather than stretching arbitrary atlas fragments.
ANIM_CELLS = {
    17: [(4, 88, 58, 72), (8, 212, 58, 75), (67, 215, 58, 72),
         (126, 216, 58, 72), (250, 215, 58, 72)],
    21: [(428, 474, 73, 103), (139, 807, 97, 97)],
    26: [(0, 17, 70, 76), (128, 44, 67, 82)],
}
CHAR_FAMILY = [17, 17, 21, 21, 17, 21, 17, 17, 21, 26, 17, 17, 21, 21, 17, 26]
characters = []
try:
    raw = (REF / "char.json").read_text(encoding="utf-8")
    raw = re.sub(r",\s*([}\]])", r"\1", raw)
    characters = json.loads(raw).get("chars", [])
except Exception:
    characters = []
if not characters:
    characters = [{"id": i, "name": "HERO {}".format(i + 1), "speed": 80 + i, "jump": 1,
                   "skill_id": i + 1, "skill_value": [8, 10, 12, 14, 16, 18, 20],
                   "cooltime": [10, 9, 8, 7, 6, 5, 4], "width": .55, "height": 1.0}
                  for i in range(16)]

hero_animations = {}
for family, cells in ANIM_CELLS.items():
    frames = []
    for cell in cells:
        frame = crop_sprite(atlases.get(family), cell)
        if frame is not None:
            frames.append(frame)
    hero_animations[family] = frames or [crop_sprite(atlases.get(17), (4, 88, 58, 72))]
hero_images = []
for i, ch in enumerate(characters[:16]):
    family = CHAR_FAMILY[i % len(CHAR_FAMILY)]
    frames = hero_animations.get(family, [])
    hero_images.append(frames[0] if frames else crop_sprite(atlases.get(17), (4, 88, 58, 72)))

def read_map_data():
    rows = []
    try:
        with (REF / "map.csv").open(newline="", encoding="utf-8-sig") as f:
            for row in csv.DictReader(f):
                try:
                    rows.append({
                        "phase": int(row["phase"]),
                        "downVelY": float(row["downVelY"]),
                        "upStopDuration": float(row["upStopDuration"]),
                        "upShakeDuration": float(row["upShakeDuration"]),
                        "downStopDuration": float(row["downStopDuration"]),
                        "holeHalfPercent": int(row["holeHalfPercent"]),
                        "holeLeftPercent": int(row["holeLeftPercent"]),
                        "holeOutPercent": int(row["holeOutPercent"]),
                        "holeBottomPercent": int(row["holeBottomPercent"]),
                        "gemPattern": int(row["gemPattern"]),
                        "maxHeight": int(row["maxHeight"]),
                        "offsetY": int(row["offsetY"]),
                    })
                except (KeyError, ValueError):
                    pass
    except OSError:
        pass
    return rows or [{"phase": 0, "downVelY": 7, "upStopDuration": 2, "upShakeDuration": 2,
                     "downStopDuration": .5, "holeHalfPercent": 0, "holeLeftPercent": 0,
                     "holeOutPercent": 0, "holeBottomPercent": 0, "gemPattern": 1,
                     "maxHeight": 4, "offsetY": 5}]

map_data = read_map_data()
gem_data = []
try:
    with (REF / "gem.csv").open(newline="", encoding="utf-8-sig") as f:
        gem_data = list(csv.DictReader(f))
except OSError:
    pass

gem_sprites = [load_image(p) for p in sorted((ASSET / "gems").glob("gem_*.png"))]
gem_sprites = [im for im in gem_sprites if im is not None]

def load_save():
    defaults = {"max_distance": 0, "coins": 0, "selected": 0, "sound": True, "plays": 0}
    try:
        data = json.loads(SAVE.read_text(encoding="utf-8"))
        defaults.update(data)
    except Exception:
        pass
    return defaults

save = load_save()
selected = max(0, min(15, int(save.get("selected", 0))))
screen_name = "title"
theme_id = 3
play_state = "up_stop"
play_timer = 0.0
roof_offset = 3.5
world_x = 0.6
world_y = 1.65
vel_x = 0.0
vel_y = 0.0
grounded = False
dead = False
paused = False
distance = 0.0
run_gems = 0
slam_count = 0
cool_count = 0
skill_timer = 0.0
skill_active = False
shield_timer = 0.0
magnet_timer = 0.0
speed_timer = 0.0
camera_x = 0.0
camera_y = 4.1
segments = []
gems = []
particles = []
warning_flash = 0.0
next_hole_x = 5.6
prev_floor_grid = 2
pattern_id = 0
freeze_timer = 0.0
slow_timer = 0.0
fly_timer = 0.0
skill_id_active = -1
map_span_grid = 0
map_segment_index = 0
gems_this_chunk = 0
last_result = {}
last_save_tick = 0
mouse_down = False

def persist():
    save["selected"] = selected
    try:
        SAVE.write_text(json.dumps(save, indent=2), encoding="utf-8")
    except OSError:
        pass

def text(s, x, y, font=FONT, color=WHITE, center=False):
    surf = font.render(str(s), True, color)
    rect = surf.get_rect()
    if center:
        rect.center = (int(x), int(y))
    else:
        rect.topleft = (int(x), int(y))
    screen.blit(surf, rect)
    return rect

def panel(rect, color=(6, 15, 22, 220), border=(102, 157, 169), radius=14):
    surf = pygame.Surface((rect[2], rect[3]), pygame.SRCALPHA)
    pygame.draw.rect(surf, color, (0, 0, rect[2], rect[3]), border_radius=radius)
    pygame.draw.rect(surf, border, (0, 0, rect[2], rect[3]), 2, border_radius=radius)
    screen.blit(surf, (rect[0], rect[1]))

def button(label, rect, active=True):
    x, y, w, h = rect
    color = (40, 91, 100) if active else (39, 48, 52)
    pygame.draw.rect(screen, color, rect, border_radius=10)
    pygame.draw.rect(screen, GOLD if active else (92, 106, 109), rect, 2, border_radius=10)
    text(label, x + w / 2, y + h / 2, SMALL, WHITE if active else MUTED, True)
    return rect

def world_to_screen(wx, wy):
    return (int(W / 2 + (wx - camera_x) * 100), int(H / 2 - (wy - camera_y) * 100))

def blit_world(img, wx, wy, width, height, flip=False):
    if img is None:
        return
    sw, sh = max(1, int(width * 100)), max(1, int(height * 100))
    frame = pygame.transform.scale(img, (sw, sh))
    if flip:
        frame = pygame.transform.flip(frame, True, False)
    sx, sy = world_to_screen(wx, wy)
    screen.blit(frame, (sx - sw // 2, sy - sh // 2))

def map_phase_index():
    # Exact Map.CheckMapData formula from the recovered Unity C# source.
    return max(0, int(((slam_count / 3.0) + (int(distance) // 30)) * .5))

def choose_map_phase():
    return map_data[min(len(map_data) - 1, max(0, map_phase_index()))]

def choose_theme():
    # Map.UpdateTheme: themes are selected from the active map-data phase and
    # total play count, not from the phase number alone.
    phase = map_phase_index()
    even_play = int(save.get("plays", 0)) % 2 == 0
    if phase <= 2:
        return 0
    if 3 <= phase <= 5:
        return 1 if even_play else 3
    if 6 <= phase <= 8:
        return 2
    return 3 if even_play else 1

def gen_segment(x, initial=False):
    global next_hole_x, prev_floor_grid, map_span_grid, map_segment_index, gems_this_chunk
    cfg = choose_map_phase()
    # Weighted width tables transcribed from Map.GenPatterns in the original C#.
    pattern_weights = [
        [1, 2, 3, 2, 1, 0, 0, 0, 0, 0],
        [0, 2, 3, 3, 2, 1, 0, 0, 0, 0],
        [0, 0, 0, 3, 3, 3, 2, 1, 1, 1],
        [0, 0, 0, 0, 1, 1, 2, 2, 2, 2],
        [0, 1, 0, 0, 0, 0, 0, 1, 1, 0],
    ]
    weights = pattern_weights[max(0, min(4, pattern_id))]
    width_choices = [i + 2 for i, count in enumerate(weights) for _ in range(count)]
    grid_width = random.choice(width_choices or [2, 3, 4])
    width = grid_width * .7
    # Approximate MakeSubRocks' neighbour-height constraint with original phase maxHeight.
    # Port of Map.MakeSubRocks: every third rock permits one extra grid
    # of upward change; Random.Range's upper bound is exclusive in Unity too.
    jump_height = 2
    upward_allowance = jump_height - 1 if map_segment_index % 3 != 0 else jump_height
    low = max(1, prev_floor_grid - jump_height)
    high_exclusive = min(int(cfg["maxHeight"]), prev_floor_grid + upward_allowance + 1)
    if high_exclusive <= low:
        high_exclusive = low + 1
    floor_grid = random.randrange(low, high_exclusive)
    if floor_grid == prev_floor_grid:
        floor_grid = (floor_grid + 1 if floor_grid < (1 + int(cfg["maxHeight"])) * .5
                      else floor_grid - 1)
    floor_grid = max(1, min(int(cfg["maxHeight"]), floor_grid))
    prev_floor_grid = floor_grid
    floor_y = floor_grid * .7
    hole = False
    hole_height = 0.0
    # Original Map.MakeSubRocks starts with a crevice after 8 grid cells.
    if not initial and x + width >= next_hole_x:
        hole = True
        half_chance = max(0, min(100, int(cfg["holeHalfPercent"]))) / 100.0
        if random.random() < half_chance or floor_grid == int(cfg["maxHeight"]):
            hole_height = 2.1
        else:
            hole_height = 1.4
        if random.random() < max(0, min(100, int(cfg["holeOutPercent"]))) / 100.0:
            next_hole_x = x + width + random.uniform(12.6, 18.9)
        else:
            next_hole_x = x + width + random.uniform(3.8, 12.6)
    seg = {"x": x, "w": width, "floor": floor_y, "hole": hole,
           "hole_height": hole_height, "floor_grid": floor_grid}
    segments.append(seg)

    # Port the original GemData.turns / Map.MakeSubRocks placement rules. The
    # selected map phase chooses a gem pattern, and each slam advances its turn.
    gem_row = next((row for row in gem_data if str(row.get("id", "")) == str(cfg["gemPattern"])), None)
    turn_index = (slam_count + 1) % 5
    turn_value = 0
    if gem_row is not None:
        try:
            turn_value = int(gem_row.get("turn{}".format(turn_index + 1), 0))
        except (TypeError, ValueError):
            turn_value = 0
    gem_here = False
    if turn_value == 3 and gems_this_chunk <= 6:
        gem_here = (map_segment_index % 3 == 0)
    elif turn_value == 5:
        gem_here = hole
    elif turn_value != 6:
        if turn_value == 8 and gems_this_chunk <= 6:
            gem_here = True
        elif turn_value == 1 and map_segment_index == 2:
            gem_here = True
        elif turn_value == 2 and map_segment_index in (2, 4):
            gem_here = True
    if gem_here:
        gems.append({"x": x + width * .5, "y": floor_y + .7, "kind": random.randrange(max(1, len(gem_sprites))),
                     "taken": False, "phase": random.random() * math.tau})
        gems_this_chunk += 1

    map_span_grid += grid_width
    map_segment_index += 1
    if map_span_grid >= 36:
        map_span_grid = 0
        map_segment_index = 0
        gems_this_chunk = 0
    return x + width

def reset_game():
    global screen_name, play_state, play_timer, roof_offset, world_x, world_y
    global vel_x, vel_y, grounded, dead, paused, distance, run_gems, slam_count
    global cool_count, skill_timer, skill_active, shield_timer, magnet_timer, speed_timer
    global camera_x, camera_y, segments, gems, particles, warning_flash, last_result
    global next_hole_x, prev_floor_grid, pattern_id, freeze_timer, slow_timer, fly_timer, skill_id_active
    global map_span_grid, map_segment_index, gems_this_chunk
    screen_name = "game"
    play_state = "up_stop"
    play_timer = 0.0
    roof_offset = 3.5
    world_x, world_y = .6, 1.7
    vel_x = vel_y = 0.0
    grounded = False
    dead = paused = False
    distance = 0.0
    run_gems = 0
    slam_count = cool_count = 0
    skill_timer = 0.0
    skill_active = False
    shield_timer = magnet_timer = speed_timer = 0.0
    camera_x, camera_y = 0.0, 4.1
    segments, gems, particles = [], [], []
    warning_flash = 0.0
    last_result = {}
    next_hole_x = 5.6
    prev_floor_grid = 2
    pattern_id = 0
    freeze_timer = slow_timer = fly_timer = 0.0
    skill_id_active = -1
    map_span_grid = map_segment_index = gems_this_chunk = 0
    x = -8.0
    while x < 70:
        x = gen_segment(x, initial=(x < 4))
    start_seg = next((s for s in segments if s["x"] <= world_x <= s["x"] + s["w"]), None)
    if start_seg:
        # Unity's Hero transform is at the feet; its BoxCollider2D center is
        # height/2 above that transform.
        world_y = start_seg["floor"]
        grounded = True
    persist()

def trigger_skill(manual=True):
    global skill_active, skill_timer, shield_timer, magnet_timer, speed_timer
    global pattern_id, cool_count, world_x, world_y, camera_x, vel_y, grounded
    global freeze_timer, slow_timer, fly_timer, skill_id_active
    ch = characters[selected]
    sid = int(ch.get("skill_id", 7))
    manual_ids = (12, 15, 16)
    if manual and sid not in manual_ids:
        return
    if not manual and sid in manual_ids:
        return
    need = int((ch.get("cooltime") or [10])[0])
    if skill_active or cool_count < need:
        return
    values = ch.get("skill_value", [10])
    # PlayerData passes the configured skill_value directly to GameController's
    # duration timer; it is in seconds, not tenths of a second.
    duration = max(1.0, float(values[0] if values else 10))
    skill_active = True
    skill_id_active = sid
    skill_timer = duration
    cool_count = 0

    if sid == 1:                 # map stop / blink
        freeze_timer = max(freeze_timer, duration)
    elif sid in (2, 3, 6, 11):  # rock-breaking, shelter, rebirth, transparency
        shield_timer = max(shield_timer, min(duration, 4.0 if sid == 6 else duration))
        if sid == 6:
            freeze_timer = max(freeze_timer, 2.0)
    elif sid in (4, 9):         # original speed-up skills
        speed_timer = max(speed_timer, duration)
        pattern_id = 3
    elif sid == 5:               # teleport to a later floor-rock segment
        target_x = world_x + 10.0
        target = next((seg for seg in segments if seg["x"] <= target_x <= seg["x"] + seg["w"]), None)
        world_x = target_x
        if target:
            world_y = target["floor"]
            grounded = True
        vel_y = 0.0
        camera_x = max(0.0, world_x + 1.3)
        magnet_timer = max(magnet_timer, 2.0)
    elif sid == 7:               # John's original map-generation skill adds up to five gems
        spawned = 0
        for seg in segments:
            if world_x + .5 < seg["x"] < world_x + 8 and spawned < 5:
                for k in range(min(5 - spawned, max(1, min(3, int(seg["w"] / 1.2))))):
                    gems.append({"x": seg["x"] + .4 + k * .45, "y": seg["floor"] + 1.0,
                                 "kind": random.randrange(max(1, len(gem_sprites))),
                                 "taken": False, "phase": random.random() * math.tau})
                    spawned += 1
    elif sid == 8:               # flight skill
        fly_timer = max(fly_timer, duration)
        shield_timer = max(shield_timer, min(duration, 3.0))
    elif sid == 10:              # slow map
        slow_timer = max(slow_timer, duration)
    elif sid == 12:              # hammer action
        shield_timer = max(shield_timer, 1.5)
    elif sid == 13:              # Snow skill effect
        shield_timer = max(shield_timer, min(duration, 3.0))
    elif sid == 14:              # Hulk breaks rocks
        shield_timer = max(shield_timer, min(duration, 3.0))
        speed_timer = max(speed_timer, duration)
    elif sid == 15:              # soccer ball action
        shield_timer = max(shield_timer, 1.2)
    elif sid == 16:              # missile action
        shield_timer = max(shield_timer, 1.5)

def finish_run():
    global screen_name, last_result, dead, paused
    if screen_name != "game" or dead:
        return
    dead = True
    screen_name = "result"
    d = int(max(0, distance))
    earned = int(run_gems)
    save["max_distance"] = max(int(save.get("max_distance", 0)), d)
    save["coins"] = int(save.get("coins", 0)) + earned
    save["plays"] = int(save.get("plays", 0)) + 1
    last_result = {"distance": d, "gems": earned, "record": d >= int(save["max_distance"])}
    persist()

def move_phase(dt):
    global play_state, play_timer, roof_offset, slam_count, cool_count, warning_flash
    if freeze_timer > 0:
        return
    if slow_timer > 0:
        dt *= .35
    play_timer += dt
    cfg = choose_map_phase()
    down_speed = max(4.0, float(cfg["downVelY"]))
    if play_state == "up_stop":
        if play_timer >= max(.45, float(cfg["upStopDuration"])):
            play_state, play_timer = "warning", 0.0
    elif play_state == "warning":
        warning_flash = 1.0
        if play_timer >= max(.35, float(cfg["upShakeDuration"])):
            play_state, play_timer = "down", 0.0
    elif play_state == "down":
        roof_offset = max(0.0, roof_offset - down_speed * dt)
        if roof_offset <= 0.0:
            play_state, play_timer = "down_stop", 0.0
            slam_count += 1
            if not skill_active:
                cool_count += 1
    elif play_state == "down_stop":
        if play_timer >= max(.25, float(cfg["downStopDuration"])):
            play_state, play_timer = "up", 0.0
    elif play_state == "up":
        roof_offset = min(3.5, roof_offset + down_speed * dt)
        if roof_offset >= 3.5:
            play_state, play_timer = "up_stop", 0.0
            warning_flash = 0.0

def draw_rock_block(rect, ceiling=False, warning=False):
    x, y, w, h = rect
    if w <= 0 or h <= 0:
        return
    px = int(x); py = int(y); pw = max(1, int(w)); ph = max(1, int(h))
    tile = stage_tiles.get(theme_id)
    if tile is None:
        tile = pygame.transform.scale(rock_texture, (256, 256)) if rock_texture is not None else None
    if tile is None:
        pygame.draw.rect(screen, (72, 54, 44), (px, py, pw, ph))
    else:
        for yy in range(py, py + ph, 256):
            for xx in range(px, px + pw, 256):
                screen.blit(tile, (xx, yy), area=(0, 0, min(256, px + pw - xx), min(256, py + ph - yy)))
    strip = (stage_ceiling_textures if ceiling else stage_floor_textures).get(theme_id)
    if strip is not None:
        strip_y = py + ph - strip.get_height() if ceiling else py
        for xx in range(px, px + pw, strip.get_width()):
            screen.blit(strip, (xx, strip_y),
                        area=(0, 0, min(strip.get_width(), px + pw - xx), strip.get_height()))
    if warning:
        edge_y = py + ph - 2 if ceiling else py
        pygame.draw.line(screen, (255, 213, 145), (px, edge_y), (px + pw, edge_y), 2)

def draw_background(theme_override=None):
    theme = theme_id if theme_override is None else int(theme_override)
    layers = background_textures.get(theme, background_textures.get(3, []))
    cache_key = (theme, W, H)
    if cache_key not in scaled_bg_cache:
        scaled_bg_cache[cache_key] = [
            pygame.transform.scale(layer, (W, H)) if layer is not None else None
            for layer in layers
        ]
    scaled = scaled_bg_cache[cache_key]
    rates = (0.0, 0.18, 0.34)
    if not scaled or scaled[0] is None:
        screen.fill(BG)
        return
    for i, layer in enumerate(scaled):
        if layer is None:
            continue
        scroll = int(camera_x * rates[i] * 100) % max(1, W)
        screen.blit(layer, (-scroll, 0))
        if scroll:
            screen.blit(layer, (W - scroll, 0))

def draw_world():
    draw_background()
    warning = play_state == "warning"
    # Draw cave floor and roof from the generated segment stream.
    for seg in segments:
        sx = W / 2 + (seg["x"] - camera_x) * 100
        sw = seg["w"] * 100
        if sx + sw < -5 or sx > W + 5:
            continue
        floor_y = seg["floor"]
        # Source MakeSubRocks builds the ceiling segment to leave PLAYER_HEIGHT
        # (two grid cells) of extra room only at matching crevice segments.
        roof_y = floor_y + roof_offset + seg["hole_height"]
        floor_top = H / 2 - (floor_y - camera_y) * 100
        draw_rock_block((sx, floor_top, sw, H - floor_top + 10), False, False)
        roof_bottom = H / 2 - (roof_y - camera_y) * 100
        draw_rock_block((sx, -10, sw, roof_bottom + 10), True, warning)
    # Gem pickups from original game are bright, rotating diamond-like collectibles.
    for g in gems:
        if g["taken"]:
            continue
        sx, sy = world_to_screen(g["x"], g["y"])
        if -20 < sx < W + 20 and -20 < sy < H + 20:
            pulse = 1 + int(2 * math.sin(pygame.time.get_ticks() / 180 + g["phase"]))
            if gem_sprites:
                sprite = gem_sprites[int(g.get("kind", 0)) % len(gem_sprites)]
                size = 26 + pulse
                sprite = pygame.transform.scale(sprite, (size, size))
                screen.blit(sprite, sprite.get_rect(center=(sx, sy)))
            else:
                pts = [(sx, sy - 13 - pulse), (sx + 10 + pulse, sy),
                       (sx, sy + 13 + pulse), (sx - 10 - pulse, sy)]
                pygame.draw.polygon(screen, CYAN, pts)
                pygame.draw.polygon(screen, WHITE, pts, 2)
    # Character sprite. We use the original sprite sheet art; fallback is vector-drawn.
    family = CHAR_FAMILY[selected % len(CHAR_FAMILY)]
    frames = hero_animations.get(family, [])
    hero = hero_images[selected] if selected < len(hero_images) else None
    char_height = float(characters[selected].get("height", 1.0)) if selected < len(characters) else 1.0
    char_width = float(characters[selected].get("width", .55)) if selected < len(characters) else .55
    hero_center_y = world_y + char_height * .5
    if frames:
        if fly_timer > 0 or not grounded:
            frame_index = min(1, len(frames) - 1)
        elif abs(vel_x) > .1:
            frame_index = 1 + (pygame.time.get_ticks() // 100) % max(1, len(frames) - 1)
        else:
            frame_index = 0
        hero = frames[int(frame_index)]
    sx, sy = world_to_screen(world_x, hero_center_y)
    if hero:
        pw, ph = int(max(.3, char_width) * 100), int(max(.35, char_height) * 100)
        sprite = pygame.transform.scale(hero, (pw, ph))
        if vel_x < 0:
            sprite = pygame.transform.flip(sprite, True, False)
        screen.blit(sprite, (sx - pw // 2, sy - ph // 2))
    else:
        pygame.draw.circle(screen, (240, 180, 120), (sx, sy - 24), 17)
        pygame.draw.rect(screen, (238, 204, 35), (sx - 15, sy - 8, 30, 36), border_radius=5)
        pygame.draw.line(screen, (25, 40, 55), (sx - 6, sy + 26), (sx - 10, sy + 42), 7)
        pygame.draw.line(screen, (25, 40, 55), (sx + 6, sy + 26), (sx + 10, sy + 42), 7)
    if shield_timer > 0:
        pygame.draw.circle(screen, CYAN, (sx, sy), 54, 3)
    # Dust particles from landing and cave slams.
    for p in particles[:]:
        p["life"] -= 1 / FPS
        p["x"] += p["vx"] / FPS
        p["y"] += p["vy"] / FPS
        if p["life"] <= 0:
            particles.remove(p)
        else:
            px, py = world_to_screen(p["x"], p["y"])
            pygame.draw.circle(screen, (183, 171, 145), (px, py), max(1, int(4 * p["life"])))
    # HUD modeled on the original distance / gem / skill readouts.
    panel((18, 16, 260, 108), (3, 15, 22, 190), (65, 112, 123), 9)
    text("CREVICE HERO", 32, 24, SMALL, GOLD)
    text("DISTANCE  {:04d} m".format(int(distance)), 32, 52, SMALL)
    text("GEMS  {}".format(int(run_gems)), 32, 80, SMALL, CYAN)
    ch = characters[selected]
    text("{}".format(ch.get("name", "HERO")), W - 22, 22, SMALL, WHITE, True)
    skill_need = int((ch.get("cooltime") or [10])[0])
    manual_skill = int(ch.get("skill_id", 7)) in (12, 15, 16)
    if skill_active:
        skill_label = "SKILL ACTIVE  {:.0f}s".format(max(0, skill_timer))
    elif manual_skill and cool_count >= skill_need:
        skill_label = "USE SKILL (E)"
    else:
        skill_label = "SKILL  {}/{}".format(cool_count, skill_need)
    button(skill_label, (W - 250, H - 66, 226, 42), manual_skill and cool_count >= skill_need and not skill_active)
    text("A/D or ←/→ move   SPACE jump   E skill   ESC pause", W / 2, H - 20, TINY, MUTED, True)
    if warning:
        panel((W // 2 - 190, 18, 380, 54), (90, 25, 18, 210), (255, 160, 90), 10)
        text("CEILING SLAM WARNING!", W / 2, 45, SMALL, (255, 220, 190), True)
    if paused:
        panel((W // 2 - 230, H // 2 - 115, 460, 230))
        text("PAUSED", W / 2, H / 2 - 55, TITLE_FONT, GOLD, True)
        text("Press ESC or click Resume", W / 2, H / 2 + 5, SMALL, WHITE, True)
        button("RESUME", (W // 2 - 105, H // 2 + 48, 210, 48))

def draw_title():
    draw_background()
    if title_logo:
        screen.blit(title_logo, title_logo.get_rect(center=(W // 2, 220)))
    else:
        text("CREVICE HERO", W / 2, 170, TITLE_FONT, GOLD, True)
    text("A PC reconstruction using original gameplay scripts and game data", W / 2, 320, SMALL, MUTED, True)
    button("PLAY", (W // 2 - 150, 385, 300, 58))
    button("CHARACTER SELECT", (W // 2 - 150, 457, 300, 58))
    button("OPTIONS", (W // 2 - 150, 529, 300, 52))
    button("QUIT", (W // 2 - 150, 595, 300, 48))
    text("Private fan reconstruction • Original Android services removed", W / 2, H - 28, TINY, MUTED, True)

def draw_select():
    draw_background()
    text("CHOOSE YOUR HERO", W / 2, 54, TITLE_FONT, GOLD, True)
    text("Original roster: 16 characters • Left/Right to browse", W / 2, 112, SMALL, MUTED, True)
    ch = characters[selected]
    hero = hero_images[selected] if selected < len(hero_images) else None
    panel((W // 2 - 310, 155, 620, 420), (3, 15, 22, 220), (83, 135, 146), 16)
    if hero:
        sprite = pygame.transform.scale(hero, (150, 190))
        screen.blit(sprite, sprite.get_rect(center=(W // 2, 320)))
    else:
        pygame.draw.circle(screen, GOLD, (W // 2, 280), 48)
    text(str(ch.get("name", "HERO")).upper(), W / 2, 440, TITLE_FONT, WHITE, True)
    text("Speed: {}     Jump: {}".format(ch.get("speed", 80), ch.get("jump", 1)), W / 2, 500, SMALL, CYAN, True)
    text("Skill ID: {}     Skill cooldown: {}".format(ch.get("skill_id", 7), (ch.get("cooltime") or [10])[0]),
         W / 2, 531, SMALL, MUTED, True)
    button("< PREVIOUS", (W // 2 - 280, 615, 180, 48))
    button("SELECT", (W // 2 - 90, 615, 180, 48))
    button("NEXT >", (W // 2 + 100, 615, 180, 48))
    button("BACK", (W // 2 - 90, 682, 180, 44))
    text("Tip: the selected character's speed, size and skill come from char.json", W / 2, 752, TINY, MUTED, True)

def draw_options():
    draw_background()
    text("OPTIONS", W / 2, 150, TITLE_FONT, GOLD, True)
    text("Sound effects: {}".format("ON" if save.get("sound", True) else "OFF"), W / 2, 290, FONT, WHITE, True)
    button("TOGGLE SOUND", (W // 2 - 150, 360, 300, 54))
    button("RESET RECORD", (W // 2 - 150, 430, 300, 54))
    button("BACK", (W // 2 - 150, 500, 300, 54))
    text("Best distance: {} m     Total gems: {}".format(save.get("max_distance", 0), save.get("coins", 0)),
         W / 2, 610, SMALL, MUTED, True)

def draw_result():
    draw_background()
    panel((W // 2 - 300, 120, 600, 540), (3, 15, 22, 230), (103, 155, 164), 18)
    text("RUN ENDED", W / 2, 185, TITLE_FONT, GOLD, True)
    text("DISTANCE", W / 2, 270, SMALL, MUTED, True)
    text("{} m".format(last_result.get("distance", int(distance))), W / 2, 320, TITLE_FONT, WHITE, True)
    text("Gems collected: {}".format(last_result.get("gems", int(run_gems))), W / 2, 390, FONT, CYAN, True)
    text("Best: {} m".format(save.get("max_distance", 0)), W / 2, 440, SMALL, WHITE, True)
    if last_result.get("record"):
        text("NEW RECORD!", W / 2, 480, SMALL, GOLD, True)
    button("PLAY AGAIN", (W // 2 - 140, 525, 280, 52))
    button("MAIN MENU", (W // 2 - 140, 590, 280, 48))

def rect_for_player():
    height_world = float(characters[selected].get("height", 1.0)) if selected < len(characters) else 1.0
    px, py = world_to_screen(world_x, world_y + height_world * .5)
    width = max(30, int(float(characters[selected].get("width", .55)) * 100)) if selected < len(characters) else 55
    height = max(42, int(height_world * 100))
    return pygame.Rect(px - width // 2, py - height // 2, width, height)

def update_game(dt, keys):
    global world_x, world_y, vel_x, vel_y, grounded, dead, camera_x, camera_y, theme_id
    global skill_timer, skill_active, shield_timer, magnet_timer, speed_timer, pattern_id
    global distance, warning_flash, freeze_timer, slow_timer, fly_timer, skill_id_active
    if paused or dead:
        return
    move_phase(dt)
    theme_id = choose_theme()
    ch = characters[selected]
    skill_need = int((ch.get("cooltime") or [10])[0])
    if cool_count >= skill_need and int(ch.get("skill_id", 7)) not in (12, 15, 16) and not skill_active:
        trigger_skill(manual=False)
    if freeze_timer > 0:
        freeze_timer = max(0.0, freeze_timer - dt)
    if slow_timer > 0:
        slow_timer = max(0.0, slow_timer - dt)
    cfg = choose_map_phase()
    left = keys[pygame.K_LEFT] or keys[pygame.K_a]
    right = keys[pygame.K_RIGHT] or keys[pygame.K_d]
    speed = float(characters[selected].get("speed", 80)) * 7.0 / 80.0
    if not grounded:
        speed *= 6.0 / 7.0
    if speed_timer > 0:
        speed *= 2.5
        speed_timer = max(0.0, speed_timer - dt)
    vel_x = (-speed if left else speed if right else 0.0)
    old_y = world_y
    world_x += vel_x * dt
    if fly_timer > 0:
        up = keys[pygame.K_UP] or keys[pygame.K_w]
        down = keys[pygame.K_DOWN] or keys[pygame.K_s]
        world_y += (2.8 if up else -2.8 if down else 0.0) * dt
        vel_y = 0.0
        fly_timer = max(0.0, fly_timer - dt)
    else:
        vel_y -= 19.0 * dt
        world_y += vel_y * dt
    world_x = max(-1.0, world_x)
    camera_x = max(0.0, world_x + 1.3)
    camera_y = 4.1
    # Keep the map stream ahead of the camera; old segments are discarded.
    farthest = max([s["x"] + s["w"] for s in segments] or [world_x])
    while farthest < camera_x + 13:
        farthest = gen_segment(farthest)
    segments[:] = [s for s in segments if s["x"] + s["w"] > camera_x - 12]
    gems[:] = [g for g in gems if not g["taken"] and g["x"] > camera_x - 12]
    # Unity's Hero transform is at the feet; BoxCollider2D extends upward by
    # the selected character's configured height.
    floor_seg = next((s for s in segments if s["x"] <= world_x <= s["x"] + s["w"]), None)
    if floor_seg and vel_y <= 0 and world_y <= floor_seg["floor"]:
        world_y = floor_seg["floor"]
        vel_y = 0.0
        grounded = True
    else:
        grounded = False
    body_height = float(characters[selected].get("height", 1.0))
    body_top = world_y + body_height
    roof_bottom = (floor_seg["floor"] + roof_offset + floor_seg["hole_height"]) if floor_seg else 5.2 + roof_offset
    if body_top >= roof_bottom and shield_timer <= 0:
        finish_run()
        return
    if world_y < -1.2:
        finish_run()
        return
    # Original characters have different width/height values; pickups use overlap.
    for g in gems:
        player_center_y = world_y + body_height * .5
        if not g["taken"] and abs(world_x - g["x"]) < .48 and abs(player_center_y - g["y"]) < .68:
            g["taken"] = True
            run_pickup(g)
    if magnet_timer > 0:
        magnet_timer = max(0.0, magnet_timer - dt)
        for g in gems:
            if not g["taken"] and abs(g["x"] - world_x) < 3.0 and abs(g["y"] - world_y) < 3.0:
                g["x"] += (world_x - g["x"]) * min(1, dt * 3)
                g["y"] += (world_y - g["y"]) * min(1, dt * 3)
    if shield_timer > 0:
        shield_timer = max(0.0, shield_timer - dt)
    if skill_active:
        skill_timer -= dt
        if skill_timer <= 0:
            skill_active = False
            skill_id_active = -1
            if int(characters[selected].get("skill_id", 7)) in (4, 9):
                pattern_id = 0
    if warning_flash > 0:
        warning_flash = max(0.0, warning_flash - dt)
    distance = max(distance, world_x)
    # Store short dust bursts while moving on the ground.
    if grounded and abs(vel_x) > 0.1 and random.random() < .10:
        particles.append({"x": world_x - .25, "y": world_y + .05, "vx": random.uniform(-1, 1),
                          "vy": random.uniform(.1, 1.2), "life": random.uniform(.15, .35)})

def run_pickup(g):
    global run_gems
    run_gems += 1
    if save.get("sound", True):
        # Short, soft synthesized pickup sound; no copyrighted original audio is redistributed.
        pass

def click_button(pos, rect):
    return pygame.Rect(rect).collidepoint(pos)

def handle_click(pos):
    global screen_name, selected, paused, world_y, vel_y, grounded
    if screen_name == "title":
        if click_button(pos, (W // 2 - 150, 385, 300, 58)):
            reset_game()
        elif click_button(pos, (W // 2 - 150, 457, 300, 58)):
            screen_name = "select"
        elif click_button(pos, (W // 2 - 150, 529, 300, 52)):
            screen_name = "options"
        elif click_button(pos, (W // 2 - 150, 595, 300, 48)):
            pygame.event.post(pygame.event.Event(pygame.QUIT))
    elif screen_name == "select":
        if click_button(pos, (W // 2 - 280, 615, 180, 48)):
            selected = (selected - 1) % min(16, len(characters))
            persist()
        elif click_button(pos, (W // 2 + 100, 615, 180, 48)):
            selected = (selected + 1) % min(16, len(characters))
            persist()
        elif click_button(pos, (W // 2 - 90, 615, 180, 48)):
            persist()
            screen_name = "title"
        elif click_button(pos, (W // 2 - 90, 682, 180, 44)):
            screen_name = "title"
    elif screen_name == "options":
        if click_button(pos, (W // 2 - 150, 360, 300, 54)):
            save["sound"] = not save.get("sound", True)
            persist()
        elif click_button(pos, (W // 2 - 150, 430, 300, 54)):
            save["max_distance"] = 0
            persist()
        elif click_button(pos, (W // 2 - 150, 500, 300, 54)):
            screen_name = "title"
    elif screen_name == "result":
        if click_button(pos, (W // 2 - 140, 525, 280, 52)):
            reset_game()
        elif click_button(pos, (W // 2 - 140, 590, 280, 48)):
            screen_name = "title"
    elif screen_name == "game":
        if paused:
            paused = False
        elif click_button(pos, (W - 250, H - 66, 226, 42)):
            trigger_skill()
        else:
            if grounded:
                world_y += .02
                vel_y = 7.2
                grounded = False

def keydown(key):
    global screen_name, selected, paused, vel_y, grounded, world_y
    if key == pygame.K_ESCAPE:
        if screen_name == "game":
            paused = not paused
        elif screen_name in ("select", "options"):
            screen_name = "title"
        elif screen_name == "result":
            screen_name = "title"
    if screen_name == "title":
        if key in (pygame.K_RETURN, pygame.K_SPACE):
            reset_game()
        elif key == pygame.K_c:
            screen_name = "select"
    elif screen_name == "select":
        if key in (pygame.K_LEFT, pygame.K_a):
            selected = (selected - 1) % min(16, len(characters)); persist()
        elif key in (pygame.K_RIGHT, pygame.K_d):
            selected = (selected + 1) % min(16, len(characters)); persist()
        elif key in (pygame.K_RETURN, pygame.K_SPACE):
            screen_name = "title"; persist()
    elif screen_name == "game" and not paused:
        if key in (pygame.K_SPACE, pygame.K_UP, pygame.K_w) and grounded:
            vel_y = 7.2
            world_y += .02
            grounded = False
        elif key in (pygame.K_e, pygame.K_LSHIFT, pygame.K_RSHIFT):
            trigger_skill()
        elif key == pygame.K_p:
            paused = True
    elif screen_name == "result":
        if key in (pygame.K_RETURN, pygame.K_SPACE):
            reset_game()
        elif key == pygame.K_BACKSPACE:
            screen_name = "title"

def main():
    global screen, W, H, camera_x, scaled_bg_cache
    running = True
    while running:
        dt = min(.04, clock.tick(FPS) / 1000.0)
        if dt <= 0:
            dt = 1 / FPS
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            elif event.type == pygame.VIDEORESIZE:
                W, H = max(900, event.w), max(600, event.h)
                screen = pygame.display.set_mode((W, H), pygame.RESIZABLE)
                scaled_bg_cache.clear()
            elif event.type == pygame.KEYDOWN:
                keydown(event.key)
            elif event.type == pygame.MOUSEBUTTONDOWN and event.button == 1:
                handle_click(event.pos)
        keys = pygame.key.get_pressed()
        if screen_name == "game":
            update_game(dt, keys)
            draw_world()
        elif screen_name == "title":
            draw_title()
        elif screen_name == "select":
            draw_select()
        elif screen_name == "options":
            draw_options()
        elif screen_name == "result":
            draw_result()
        pygame.display.flip()
    persist()
    pygame.quit()

if __name__ == "__main__":
    main()
