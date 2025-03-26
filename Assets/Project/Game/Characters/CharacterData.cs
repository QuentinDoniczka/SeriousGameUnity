namespace Project.Game.Characters
{
    public class CharacterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Faction { get; set; }
        public string Role { get; set; }
        
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Mana { get; set; }
        public int Experience { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, Nom: {Name}, Faction: {Faction}, Rôle: {Role}, " +
                   $"Santé: {Health}, Attaque: {Attack}, Défense: {Defense}, " +
                   $"Mana: {Mana}, Expérience: {Experience}";
        }
    }
}