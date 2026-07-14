namespace Zoo
{
    public enum CombatResult
    {
        None,
        AttackerWinsClean,
        AttackerWinsWithInjury,
    }

    public static class AttackResolver
    {
        public static CombatResult Resolve(Unit attacker, Unit defender)
        {
            if (attacker.HealthCurrent <= 0 ||
                defender.HealthCurrent <= 0)
            {
                return CombatResult.None;
            }

            if (attacker.Consumption != ConsumptionType.Predator)
            {
                return CombatResult.None;
            }

            if (defender.Consumption == ConsumptionType.Prey)
            {
                return CombatResult.AttackerWinsClean;
            }

            if (attacker.Rank > defender.Rank)
            {
                return CombatResult.AttackerWinsClean;
            }

            if (attacker.Rank == defender.Rank &&
                attacker.HealthCurrent >= defender.HealthCurrent)
            {
                return CombatResult.AttackerWinsWithInjury;
            }

            return CombatResult.None;
        }
    }
}
