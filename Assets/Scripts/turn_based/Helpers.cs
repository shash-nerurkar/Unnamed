using System.Collections.Generic;

public class PromptStrings {
    public static Dictionary<PlayerAttacks, string> playerAttackPrompts = new(){
        {PlayerAttacks.Attack1, "Player just used attack 1"},
        {PlayerAttacks.Attack2, "Player just used attack 2"},
        {PlayerAttacks.Attack3, "Player just used attack 3"},
        {PlayerAttacks.Attack4, "Player just used attack 4"}
    };
}

public enum GameStates {
    PlayerSelectingAttack,
    PlayerAttackingText,
    PlayerAttacking,
    EnemyDyingText,
    EnemyDying,
    EnemyAttackingText,
    EnemyAttacking,
    PlayerDyingText,
    PlayerDying,
    GameEnd,
}

public enum PlayerAttacks {
    Attack1 = 0,
    Attack2 = 1,
    Attack3 = 2,
    Attack4 = 3,
}

public enum EnemyAttacks {
    Attack1 = 0,
    Attack2 = 1,
    Attack3 = 2,
    Attack4 = 3,
}
