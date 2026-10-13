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

    def test_game_loop_state_and_render(self):
        game.reset_game()
        keys = pygame.key.get_pressed()
        for _ in range(20):
            game.update_game(1 / 60, keys)
        game.draw_world()
        self.assertGreater(len(game.segments), 0)
        self.assertGreaterEqual(game.distance, 0)
        pygame.display.flip()

if __name__ == "__main__":
    unittest.main()
