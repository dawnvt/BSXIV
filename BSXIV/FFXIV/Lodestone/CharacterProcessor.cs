using Newtonsoft.Json;
using BSXIV.Utilities;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BSXIV.FFXIV.Lodestone
{
    public class CharacterProcessor
    {
        private LodestoneRequester _lodestoneRequester;
        private ILogger _logging;
        private DbContext _dbContext;
        private IDatabase _redis;

        public CharacterProcessor(ILogger<CharacterProcessor> logging, LodestoneRequester lodestoneRequester, DbContext dbContext, ConnectionMultiplexer connectionMultiplexer)
        {
            _logging = logging;
            _lodestoneRequester = lodestoneRequester;
            _dbContext = dbContext;
            _redis = connectionMultiplexer.GetDatabase(0);
        }

        //TODO: add CSS Selector load from https://github.com/xivapi/lodestone-css-selectors and store in redis for cache
    }

    #region Lodestone Selector Classes (Generated)
    public class LodestoneCharacterSelectors
    {
        [JsonProperty("ACTIVE_CLASSJOB")]
        public LodestoneSelectors ActiveClassjob { get; set; }

        [JsonProperty("ACTIVE_CLASSJOB_LEVEL")]
        public LodestoneSelectors ActiveClassjobLevel { get; set; }

        [JsonProperty("AVATAR")]
        public LodestoneSelectors Avatar { get; set; }

        [JsonProperty("BIO")]
        public LodestoneSelectors Bio { get; set; }

        [JsonProperty("CLASSJOB_ICONS")]
        public ClassjobIcons ClassjobIcons { get; set; }

        [JsonProperty("FREE_COMPANY")]
        public FreeCompany FreeCompany { get; set; }

        [JsonProperty("GRAND_COMPANY")]
        public LodestoneSelectors GrandCompany { get; set; }

        [JsonProperty("GUARDIAN_DEITY")]
        public GuardianDeity GuardianDeity { get; set; }

        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }

        [JsonProperty("NAMEDAY")]
        public LodestoneSelectors Nameday { get; set; }

        [JsonProperty("PORTRAIT")]
        public LodestoneSelectors Portrait { get; set; }

        [JsonProperty("PVP_TEAM")]
        public FreeCompany PvpTeam { get; set; }

        [JsonProperty("RACE_CLAN_GENDER")]
        public LodestoneSelectors RaceClanGender { get; set; }

        [JsonProperty("SERVER")]
        public LodestoneSelectors Server { get; set; }

        [JsonProperty("TITLE")]
        public LodestoneSelectors Title { get; set; }

        [JsonProperty("TOWN")]
        public GuardianDeity Town { get; set; }

        [JsonProperty("STRENGTH")]
        public LodestoneSelectors Strength { get; set; }

        [JsonProperty("DEXTERITY")]
        public LodestoneSelectors Dexterity { get; set; }

        [JsonProperty("VITALITY")]
        public LodestoneSelectors Vitality { get; set; }

        [JsonProperty("INTELLIGENCE")]
        public LodestoneSelectors Intelligence { get; set; }

        [JsonProperty("MIND")]
        public LodestoneSelectors Mind { get; set; }

        [JsonProperty("CRITICAL_HIT_RATE")]
        public LodestoneSelectors CriticalHitRate { get; set; }

        [JsonProperty("DETERMINATION")]
        public LodestoneSelectors Determination { get; set; }

        [JsonProperty("DIRECT_HIT_RATE")]
        public LodestoneSelectors DirectHitRate { get; set; }

        [JsonProperty("DEFENSE")]
        public LodestoneSelectors Defense { get; set; }

        [JsonProperty("MAGIC_DEFENSE")]
        public LodestoneSelectors MagicDefense { get; set; }

        [JsonProperty("ATTACK_POWER")]
        public LodestoneSelectors AttackPower { get; set; }

        [JsonProperty("SKILL_SPEED")]
        public LodestoneSelectors SkillSpeed { get; set; }

        [JsonProperty("ATTACK_MAGIC_POTENCY")]
        public LodestoneSelectors AttackMagicPotency { get; set; }

        [JsonProperty("HEALING_MAGIC_POTENCY")]
        public LodestoneSelectors HealingMagicPotency { get; set; }

        [JsonProperty("SPELL_SPEED")]
        public LodestoneSelectors SpellSpeed { get; set; }

        [JsonProperty("TENACITY")]
        public LodestoneSelectors Tenacity { get; set; }

        [JsonProperty("PIETY")]
        public LodestoneSelectors Piety { get; set; }

        [JsonProperty("HP")]
        public LodestoneSelectors Hp { get; set; }

        [JsonProperty("MP_GP_CP")]
        public LodestoneSelectors MpGpCp { get; set; }

        [JsonProperty("MP_GP_CP_PARAMETER_NAME")]
        public LodestoneSelectors MpGpCpParameterName { get; set; }

        [JsonProperty("MAINHAND")]
        public Body Mainhand { get; set; }

        [JsonProperty("OFFHAND")]
        public Body Offhand { get; set; }

        [JsonProperty("HEAD")]
        public Body Head { get; set; }

        [JsonProperty("BODY")]
        public Body Body { get; set; }

        [JsonProperty("HANDS")]
        public Body Hands { get; set; }

        [JsonProperty("WAIST")]
        public Body Waist { get; set; }

        [JsonProperty("LEGS")]
        public Body Legs { get; set; }

        [JsonProperty("FEET")]
        public Body Feet { get; set; }

        [JsonProperty("EARRINGS")]
        public Body Earrings { get; set; }

        [JsonProperty("NECKLACE")]
        public Body Necklace { get; set; }

        [JsonProperty("BRACELETS")]
        public Body Bracelets { get; set; }

        [JsonProperty("RING1")]
        public Body Ring1 { get; set; }

        [JsonProperty("RING2")]
        public Body Ring2 { get; set; }

        [JsonProperty("SOULCRYSTAL")]
        public Soulcrystal Soulcrystal { get; set; }

        [JsonProperty("BOZJA")]
        public Bozja Bozja { get; set; }

        [JsonProperty("EUREKA")]
        public Eureka Eureka { get; set; }

        [JsonProperty("PALADIN")]
        public ClassJob Paladin { get; set; }

        [JsonProperty("WARRIOR")]
        public ClassJob Warrior { get; set; }

        [JsonProperty("DARKKNIGHT")]
        public ClassJob Darkknight { get; set; }

        [JsonProperty("GUNBREAKER")]
        public ClassJob Gunbreaker { get; set; }

        [JsonProperty("WHITEMAGE")]
        public ClassJob Whitemage { get; set; }

        [JsonProperty("SCHOLAR")]
        public ClassJob Scholar { get; set; }

        [JsonProperty("ASTROLOGIAN")]
        public ClassJob Astrologian { get; set; }

        [JsonProperty("SAGE")]
        public ClassJob Sage { get; set; }

        [JsonProperty("MONK")]
        public ClassJob Monk { get; set; }

        [JsonProperty("DRAGOON")]
        public ClassJob Dragoon { get; set; }

        [JsonProperty("NINJA")]
        public ClassJob Ninja { get; set; }

        [JsonProperty("SAMURAI")]
        public ClassJob Samurai { get; set; }

        [JsonProperty("REAPER")]
        public ClassJob Reaper { get; set; }

        [JsonProperty("BARD")]
        public ClassJob Bard { get; set; }

        [JsonProperty("MACHINIST")]
        public ClassJob Machinist { get; set; }

        [JsonProperty("DANCER")]
        public ClassJob Dancer { get; set; }

        [JsonProperty("BLACKMAGE")]
        public ClassJob Blackmage { get; set; }

        [JsonProperty("SUMMONER")]
        public ClassJob Summoner { get; set; }

        [JsonProperty("REDMAGE")]
        public ClassJob Redmage { get; set; }

        [JsonProperty("BLUEMAGE")]
        public ClassJob Bluemage { get; set; }

        [JsonProperty("CARPENTER")]
        public ClassJob Carpenter { get; set; }

        [JsonProperty("BLACKSMITH")]
        public ClassJob Blacksmith { get; set; }

        [JsonProperty("ARMORER")]
        public ClassJob Armorer { get; set; }

        [JsonProperty("GOLDSMITH")]
        public ClassJob Goldsmith { get; set; }

        [JsonProperty("LEATHERWORKER")]
        public ClassJob Leatherworker { get; set; }

        [JsonProperty("WEAVER")]
        public ClassJob Weaver { get; set; }

        [JsonProperty("ALCHEMIST")]
        public ClassJob ClassJob { get; set; }

        [JsonProperty("CULINARIAN")]
        public ClassJob Culinarian { get; set; }

        [JsonProperty("MINER")]
        public ClassJob Miner { get; set; }

        [JsonProperty("BOTANIST")]
        public ClassJob Botanist { get; set; }

        [JsonProperty("FISHER")]
        public ClassJob Fisher { get; set; }

        [JsonProperty("MINIONS")]
        public MIMO Minions { get; set; }

        [JsonProperty("MOUNTS")]
        public MIMO Mounts { get; set; }

        [JsonProperty("TOTAL")]
        public LodestoneSelectors Total { get; set; }
    }

    public class MIMO
    {
        [JsonProperty("ROOT")]
        public LodestoneSelectors Root { get; set; }

        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }

        [JsonProperty("ICON")]
        public LodestoneSelectors Icon { get; set; }
    }

    public class ClassJob
    {
        [JsonProperty("LEVEL")]
        public LodestoneSelectors Level { get; set; }

        [JsonProperty("UNLOCKSTATE")]
        public LodestoneSelectors Unlockstate { get; set; }

        [JsonProperty("EXP")]
        public LodestoneSelectors Exp { get; set; }
    }

    public class Bozja
    {
        [JsonProperty("LEVEL")]
        public LodestoneSelectors Level { get; set; }

        [JsonProperty("METTLE")]
        public LodestoneSelectors Mettle { get; set; }

        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }
    }

    public class Eureka
    {
        [JsonProperty("EXP")]
        public LodestoneSelectors Exp { get; set; }

        [JsonProperty("LEVEL")]
        public LodestoneSelectors Level { get; set; }

        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }
    }

    public class Body
    {
        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }

        [JsonProperty("DB_LINK")]
        public LodestoneSelectors DbLink { get; set; }

        [JsonProperty("MIRAGE_NAME")]
        public LodestoneSelectors MirageName { get; set; }

        [JsonProperty("MIRAGE_DB_LINK")]
        public LodestoneSelectors MirageDbLink { get; set; }

        [JsonProperty("STAIN")]
        public LodestoneSelectors Stain { get; set; }

        [JsonProperty("MATERIA_1")]
        public LodestoneSelectors Materia1 { get; set; }

        [JsonProperty("MATERIA_2")]
        public LodestoneSelectors Materia2 { get; set; }

        [JsonProperty("MATERIA_3")]
        public LodestoneSelectors Materia3 { get; set; }

        [JsonProperty("MATERIA_4")]
        public LodestoneSelectors Materia4 { get; set; }

        [JsonProperty("MATERIA_5")]
        public LodestoneSelectors Materia5 { get; set; }

        [JsonProperty("CREATOR_NAME")]
        public LodestoneSelectors CreatorName { get; set; }
    }

    public class Soulcrystal
    {
        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }
    }

    public class ClassjobIcons
    {
        [JsonProperty("ROOT")]
        public LodestoneSelectors Root { get; set; }
        [JsonProperty("ICON")]
        public LodestoneSelectors Icon { get; set; }
    }

    public class FreeCompany
    {
        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }
        [JsonProperty("ICON_LAYERS")]
        public IconLayers IconLayers { get; set; }
    }

    public class IconLayers
    {
        [JsonProperty("BOTTOM")]
        public LodestoneSelectors Bottom { get; set; }
        [JsonProperty("MIDDLE")]
        public LodestoneSelectors Middle { get; set; }
        [JsonProperty("TOP")]
        public LodestoneSelectors Top { get; set; }
    }

    public class GuardianDeity
    {
        [JsonProperty("NAME")]
        public LodestoneSelectors Name { get; set; }
        [JsonProperty("ICON")]
        public LodestoneSelectors Icon { get; set; }
    }
    #endregion
}
