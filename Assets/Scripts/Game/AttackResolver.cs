namespace Zoo
{
    public enum AttackResult
    {
        None,
        AttackerWinsClean,
        AttackerWinsWithInjury,
    }

    public static class AttackResolver
    {
        public static AttackResult Resolve(Unit attacker, Unit defender)
        {
            if (attacker.HealthCurrent <= 0 ||
                defender.HealthCurrent <= 0)
            {
                return AttackResult.None;
            }

            if (attacker.Consumption != ConsumptionType.Predator)
            {
                return AttackResult.None;
            }

            if (defender.Consumption == ConsumptionType.Prey)
            {
                return AttackResult.AttackerWinsClean;
            }

            if (attacker.Rank > defender.Rank)
            {
                return AttackResult.AttackerWinsClean;
            }

            if (attacker.Rank == defender.Rank &&
                attacker.HealthCurrent >= defender.HealthCurrent)
            {
                return AttackResult.AttackerWinsWithInjury;
            }

            return AttackResult.None;
        }
    }
}
