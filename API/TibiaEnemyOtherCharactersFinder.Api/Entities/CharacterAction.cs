using System.ComponentModel.DataAnnotations.Schema;

namespace TibiaEnemyOtherCharactersFinder.Api.Entities
{
    public class CharacterAction
    {
        public int CharacterActionId { get; set; }
        public string CharacterName { get; set; }
        public int WorldScanId { get; set; }
        public bool IsOnline { get; set; }
        public short WorldId { get; set; }
        public WorldScan WorldScan { get; set; }
        public World World { get; set; }

    }
}
