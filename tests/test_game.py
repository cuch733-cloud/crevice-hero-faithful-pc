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

    def test_original_map_phases_loaded(self):
        self.assertGreaterEqual(len(game.map_data), 10)
        self.assertEqual(game.map_data[0]["phase"], 0)
        self.assertGreater(game.map_data[0]["downVelY"], 0)

    def test_matching_crevice_survives_ceiling_slam(self):
        game.reset_game()
        crevice = next(s for s in game.segments if s["hole"])
        game.world_x = crevice["x"] + crevice["w"] * .5
        game.world_y = crevice["floor"] + .48
        game.grounded = True
        game.camera_x = game.world_x + 1.3
        game.play_state = "down"
        game.roof_offset = .1
        game.play_timer = 0.0
        game.update_game(.04, pygame.key.get_pressed())
        self.assertEqual(game.screen_name, "game")
        self.assertEqual(game.roof_offset, 0.0)

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
