import os
os.environ.setdefault("SDL_VIDEODRIVER", "dummy")
os.environ.setdefault("SDL_AUDIODRIVER", "dummy")
import unittest
import pygame
import crevice_hero as game

class GameSmokeTests(unittest.TestCase):
    def test_original_roster_loaded(self):
        self.assertEqual(len(game.characters), 16)
        self.assertEqual(game.characters[0]["name"], "JOHN")
        self.assertEqual(game.characters[1]["name"], "GIRL")

    def test_original_gem_pattern_resource_loaded(self):
        self.assertEqual(len(game.gem_data), 5)
        row = next(r for r in game.gem_data if r.get("id") == "1")
        self.assertEqual([int(row["turn{}".format(i)]) for i in range(1, 6)], [1, 1, 2, 3, 3])

    def test_original_map_phases_loaded(self):
        self.assertGreaterEqual(len(game.map_data), 10)
        self.assertEqual(game.map_data[0]["phase"], 0)
        self.assertGreater(game.map_data[0]["downVelY"], 0)

    def test_matching_crevice_survives_ceiling_slam(self):
        game.reset_game()
        crevice = next(s for s in game.segments if s["hole"])
        game.world_x = crevice["x"] + crevice["w"] * .5
        game.world_y = crevice["floor"]
        game.grounded = True
        game.camera_x = game.world_x + 1.3
        game.play_state = "down"
        game.roof_offset = .1
        game.play_timer = 0.0
        game.update_game(.04, pygame.key.get_pressed())
        self.assertEqual(game.screen_name, "game")
        self.assertEqual(game.roof_offset, 0.0)

    def test_original_skill_duration_and_auto_activation(self):
        game.reset_game()
        game.selected = 0  # JOHN: original skill ID 7 adds gems.
        game.cool_count = int((game.characters[0].get("cooltime") or [10])[0])
        before = len(game.gems)
        game.trigger_skill(manual=False)
        self.assertTrue(game.skill_active)
        self.assertEqual(game.skill_timer, float(game.characters[0]["skill_value"][0]))
        self.assertEqual(game.cool_count, 0)
        self.assertGreater(len(game.gems), before)

    def test_manual_character_skill_requires_player_action(self):
        game.reset_game()
        game.selected = 15  # ROBOT uses original skill ID 16 (missile).
        game.cool_count = int((game.characters[15].get("cooltime") or [10])[0])
        game.trigger_skill(manual=False)
        self.assertFalse(game.skill_active)
        game.trigger_skill(manual=True)
        self.assertTrue(game.skill_active)
        self.assertEqual(game.skill_id_active, 16)

    def test_game_loop_state_and_render(self):
        game.reset_game()
        keys = pygame.key.get_pressed()
        for _ in range(20):
            game.update_game(1 / 60, keys)
        game.draw_world()
        self.assertGreater(len(game.segments), 0)
        self.assertTrue(any(s["hole"] and s["hole_height"] in (1.4, 2.1) for s in game.segments))
        self.assertTrue(all(s["floor_grid"] >= 1 for s in game.segments))
        self.assertGreaterEqual(game.distance, 0)
        pygame.display.flip()

if __name__ == "__main__":
    unittest.main()
