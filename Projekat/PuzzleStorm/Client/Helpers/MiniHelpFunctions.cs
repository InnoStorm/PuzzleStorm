using StormCommonData.Enums;

namespace Client {

    static class MiniHelpFunctions {

        public static string CastDifficulty(PuzzleDifficulty level) {
            switch (level) {
                case PuzzleDifficulty.Easy:
                    return 16.ToString();
                case PuzzleDifficulty.Medium:
                    return 25.ToString();
                case PuzzleDifficulty.Hard:
                    return 36.ToString();
                default:
                    return "X";
            }
        }

        public static PuzzleDifficulty StringToDifficulty(string lvl)
        {
            switch (lvl)
            {
                case "16":
                    return PuzzleDifficulty.Easy;
                case "25":
                    return PuzzleDifficulty.Medium;
                case "36":
                    return PuzzleDifficulty.Hard;
                default:
                    return PuzzleDifficulty.Easy;
            }
        }

        public static int DifficultyToIndex(string lvl)
        {
            switch (lvl)
            {
                case "16":
                    return 0;
                case "25":
                    return 1;
                case "36":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
