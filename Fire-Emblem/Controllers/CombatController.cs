using Fire_Emblem.Combat;
using Fire_Emblem.Configuration;
using Fire_Emblem.Model;
using Fire_Emblem.Utilities;

namespace Fire_Emblem.Controllers;

public class CombatController {
    private GameView _gameView;
    private WeaponTriangleBonus _weaponTriangleBonus;
    private DamageCalculator _damageCalculator;
    public CombatController(GameView gameView, WeaponTriangleBonus weaponTriangleBonus, DamageCalculator damageCalculator) {
        _gameView = gameView;
        _weaponTriangleBonus = weaponTriangleBonus;
        _damageCalculator = damageCalculator;
    }
    public void StartCombat(Unit attacker, Unit defender) {
        _weaponTriangleBonus.ShowUnitAdvantage(_gameView, attacker, defender);
        PerformAttacks(attacker, defender);
        _gameView.ShowCombatResult(attacker, defender);
    }
    
    
    private void PerformAttacks(Unit attacker , Unit defender) {
        PerformAttack(attacker, defender);
        PerformAttack(defender, attacker);
        PerformFollowUpAttack(attacker, defender);
    }
    private void PerformAttack(Unit attacker, Unit defender) {
        if(!CanPerformAttack(attacker, defender)) return;
        int damage = _damageCalculator.CalculateDamage(attacker, defender);
        defender.ReceiveDamage(damage);
        _gameView.ShowAttackInformation(attacker, defender, damage);
    }
    private bool CanPerformAttack(Unit attacker, Unit defender) {
        return attacker.IsAlive() && defender.IsAlive();
    }
    
    private void PerformFollowUpAttack(Unit attacker, Unit defender) {
        if (!CanPerformAttack(attacker, defender)) {return;}
        if (GetEffectiveSpeed(attacker) >= GetEffectiveSpeed(defender) + GameConfig.FollowUpSpeedDifferenceRequirement ) {
            PerformAttack(attacker, defender);
        }
        else if (GetEffectiveSpeed(defender) >= GetEffectiveSpeed(attacker) + GameConfig.FollowUpSpeedDifferenceRequirement ) {
            PerformAttack(defender, attacker);
        }
        else{
            _gameView.SayThatNoUnitCanDoAFollowUp();
        }
    }

    private int GetEffectiveSpeed(Unit unit) {
        return unit.Stats.BaseStats[StatType.Spd] + unit.Stats.Bonuses[EffectPhase.Always][StatType.Spd];
    }
}