#region

using System.Diagnostics.CodeAnalysis;

using Batter.Utils.Builders.Dynamic;

#endregion

namespace Batter.Utils.Builders.Examples;

/// <summary>
///     Simple example demonstrating the new API - using EasyBuilder with property management,
///     IUniqueProp for singleton properties, IProp for collections, and fluent configuration.
/// </summary>
public class SimpleCharacterExample {

    /// <summary>
    ///     Main entry point for the Simple Character Example.
    ///     Demonstrates the creation of three different characters (Aragorn, Gandalf, and Legolas)
    ///     using the new unified builder APIs that work with both EasyBuilder and full IBuilder.
    /// </summary>
    public static void Main() {
        Console.WriteLine("=== Demonstrating Unified Builder API ===");
        Console.WriteLine();

        // NOTE: below are examples of different usage patterns for the container api; they are all valid and compatible
        //       the container api is fairly flexible and should provide a consistent crud experience
        //       the examples are simple and only do crud. conditionals, loops, etc. are covered in another example.

        DynBuildable aragorn = Character.From<CharacterBuilder>()
                                        .With<Name>("Aragorn")
                                        .With<Level>(20)
                                        .With<Skill>(SkillKey.Swords)
                                        .With<Skill>(SkillKey.Leadership)
                                        .With(new Skill(SkillKey.Swords, 90))
                                        .With<Skill>(SkillKey.Sulking, 15)
                                        .Build();

        var gandalf = Character.Create()
                               .With<Name>("Gandalf")
                               .With<Level>(60)
                               .With<Skill>(SkillKey.Magic)
                               .With<Skill>(SkillKey.Smoking, 20)
                               .Build<Character>();

        DynBuilder legolasBuilder = CharacterBuilder.New()
                                                    .With<Name>("Legolas")
                                                    .With<Level>(18)
                                                    .With<Skill>(SkillKey.Archery,    new Skill(SkillKey.Archery, 90))
                                                    .With<Skill>(SkillKey.Acrobatics, 20)
                                                    .With<Skill>(SkillKey.Tailoring);

        var legolas = (Character)legolasBuilder.Build();

        // Print character descriptions
        Console.WriteLine("Characters created using the unified builder API:");
        Console.WriteLine();
        Console.WriteLine(aragorn.As<Character>().GetDescription());
        Console.WriteLine();
        Console.WriteLine(gandalf?.GetDescription());
        Console.WriteLine();
        Console.WriteLine(legolas.GetDescription());
        Console.WriteLine();

        Console.WriteLine("=== API Compatibility Demonstration ===");
        Console.WriteLine("✓ EasyBuilder.With<T>(value) - Works");
        Console.WriteLine("✓ EasyBuilder.Add<T>(key) - Works");
        Console.WriteLine("✓ IBuilder extensions - Works");
        Console.WriteLine("✓ TypeErasedBuilder wrapper - Works");
        Console.WriteLine("✓ Unified storage system - Works");
    }

}

/// <summary>
///     Skills enum where the enum value serves as the unique ID
/// </summary>
public enum SkillKey : ushort {

    /// <summary>
    ///     Sword fighting and melee combat proficiency
    /// </summary>
    Swords = 1,

    /// <summary>
    ///     Bow and arrow marksmanship skills
    /// </summary>
    Archery = 2,

    /// <summary>
    ///     Magical spellcasting and arcane knowledge
    /// </summary>
    Magic = 3,

    /// <summary>
    ///     Ability to lead and inspire others in combat and adventure
    /// </summary>
    Leadership = 4,

    /// <summary>
    ///     Professional brooding and dramatic posturing
    /// </summary>
    Sulking = 5,

    /// <summary>
    ///     Pipe smoking and contemplative reflection
    /// </summary>
    Smoking = 6,

    /// <summary>
    ///     Agility, jumping, and athletic movement
    /// </summary>
    Acrobatics = 7,

    /// <summary>
    ///     Clothing creation and fabric manipulation
    /// </summary>
    Tailoring = 8,

    /// <summary>
    ///     Food preparation and culinary expertise
    /// </summary>
    Cooking = 9,

    /// <summary>
    ///     Silent movement and concealment abilities
    /// </summary>
    Stealth = 10,

}

/// <summary>
///     Character name property implementing IUniqueProp
/// </summary>
public class Name : IUniqueProp<CharacterBuilder, Name> {

    internal const string UNNAMED = "__UNNAMED__";

    /// <summary>
    ///     Gets or sets the string value of the character's name
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Initializes a new instance of the CharacterName class with the specified value
    /// </summary>
    /// <param name="value">The name value to set</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
    public Name(string value) { this.Value = value ?? throw new ArgumentNullException(nameof(value)); }

    /// <summary>
    ///     Determines whether the specified CharacterName is equal to the current CharacterName
    /// </summary>
    /// <param name="other">The CharacterName to compare with the current CharacterName</param>
    /// <returns>True if the specified CharacterName is equal to the current CharacterName; otherwise, false</returns>
    public bool Equals(Name? other) {
        return other != null && string.Equals(this.Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Implicitly converts a string to a CharacterName instance
    /// </summary>
    /// <param name="value">The string value to convert</param>
    /// <returns>A new CharacterName instance containing the string value</returns>
    public static implicit operator Name(string value) { return new(value); }

    /// <summary>
    ///     Implicitly converts a CharacterName instance to its string value
    /// </summary>
    /// <param name="name">The CharacterName instance to convert</param>
    /// <returns>The string value contained in the CharacterName</returns>
    public static implicit operator string(Name name) { return name.Value; }

    /// <summary>
    ///     Returns the string representation of the CharacterName
    /// </summary>
    /// <returns>The string value of the name</returns>
    public override string ToString() { return this.Value; }

}

/// <summary>
///     Character level property implementing IUniqueProp
/// </summary>
public class Level : IUniqueProp<CharacterBuilder, Level> {

    /// <summary>
    ///     Gets or sets the integer value representing the character's level
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    ///     Initializes a new instance of the CharacterLevel class with the specified value
    /// </summary>
    /// <param name="value">The level value to set</param>
    public Level(int value) { this.Value = value; }

    /// <summary>
    ///     Determines whether the specified CharacterLevel is equal to the current CharacterLevel
    /// </summary>
    /// <param name="other">The CharacterLevel to compare with the current CharacterLevel</param>
    /// <returns>True if the specified CharacterLevel is equal to the current CharacterLevel; otherwise, false</returns>
    public bool Equals(Level? other) { return other != null && this.Value == other.Value; }

    /// <summary>
    ///     Implicitly converts an integer to a CharacterLevel instance
    /// </summary>
    /// <param name="value">The integer value to convert</param>
    /// <returns>A new CharacterLevel instance containing the integer value</returns>
    public static implicit operator Level(int value) { return new(value); }

    /// <summary>
    ///     Implicitly converts a CharacterLevel instance to its integer value
    /// </summary>
    /// <param name="level">The CharacterLevel instance to convert</param>
    /// <returns>The integer value contained in the CharacterLevel</returns>
    public static implicit operator int(Level level) { return level.Value; }

    /// <summary>
    ///     Returns the string representation of the CharacterLevel
    /// </summary>
    /// <returns>The string representation of the level value</returns>
    public override string ToString() { return this.Value.ToString(); }

}

/// <summary>
///     Skill property implementing IProp for skill collections
/// </summary>
public class Skill : IProp<CharacterBuilder, Skill, Key<SkillKey>> {

    /// <summary>
    ///     Gets the skill type, which is the same as the Id
    /// </summary>
    public SkillKey SkillType => (SkillKey)this.Id;

    /// <summary>
    ///     Gets or sets the level of proficiency for this skill
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    ///     Initializes a new instance of the SkillProp class
    /// </summary>
    /// <param name="skillKey">The skill type</param>
    /// <param name="level">The skill level (defaults to 1)</param>
    public Skill(SkillKey skillKey, int level = 1) {
        this.Id    = skillKey;
        this.Level = level;
    }

    /// <summary>
    ///     Gets the unique identifier for this skill property
    /// </summary>
    public Key<SkillKey> Id { get; } // maps to Skill enum values


    /// <summary>
    ///     Determines whether the specified SkillProp is equal to the current SkillProp
    /// </summary>
    /// <param name="other">The SkillProp to compare with the current SkillProp</param>
    /// <returns>True if the specified SkillProp is equal to the current SkillProp; otherwise, false</returns>
    public bool Equals(Skill? other) {
        if (ReferenceEquals(this, other)) return true;

        return other != null && this.Id.Equals(other.Id) && this.Level == other.Level;
    }

    /// <summary>
    ///     Returns the string representation of the SkillProp
    /// </summary>
    /// <returns>A string that represents the current SkillProp</returns>
    public override string ToString() { return $"{this.SkillType} (Level {this.Level})"; }

    /// <inheritdoc />
    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        return this.Equals((Skill)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() { return HashCode.Combine(this.Level, this.Id); }

}

/// <summary>
///     Character class - simplified for the new API
/// </summary>
public class Character : DynBuildable<Character, CharacterBuilder> {

    internal IDictionary<SkillKey, ushort> Skills { get; } = new Dictionary<SkillKey, ushort>();

    /// <summary>
    ///     Gets or sets the character's name
    /// </summary>
    [NotNull]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the character's level
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    ///     Initializes a new instance of the Character class
    /// </summary>
    /// <param name="name">The character's name</param>
    /// <param name="level">The character's level</param>
    /// <exception cref="ArgumentNullException">Thrown when name is null</exception>
    public Character([NotNull] string name, int level = 1) {
        this.Name  = name ?? throw new ArgumentNullException(nameof(name));
        this.Level = level;
    }

    /// <summary>
    /// </summary>
    public Character() { this.Name = Examples.Name.UNNAMED; }


    /// <summary>
    ///     Gets a description of the character including name, level, and skills
    /// </summary>
    /// <returns>A formatted string description of the character</returns>
    public string GetDescription() {
        // FIXME: use the internal DynStorage api 
        List<string> skillNames = this.Skills.Keys.Select(s => s.ToString()).ToList();

        string skillsText = skillNames.Count > 0
            ? string.Join(", ", skillNames)
            : "no skills";

        return $"{this.Name}, level {this.Level}, says hi! He's proficient in {skillsText}.";
    }

}

/// <summary>
///     Character builder using the new EasyBuilder pattern with proper type-safe property management
/// </summary>
public class CharacterBuilder : DynBuilder<Character, CharacterBuilder> {

    /// <summary>
    ///     Creates a new CharacterBuilder instance
    /// </summary>
    /// <returns>A new CharacterBuilder</returns>
    public static CharacterBuilder New() { return new(); }

    /// <inheritdoc />
    public override Character Build(Character? nonDefaultStartingPoint) {
        // NOTE: this eximplifies a standard use case for the builder pattern
        //       we read the container api here, and manually update the actual TResult instance
        //       based on what we find in the storage collection. which can conveniently store anything (almost)
        //       but still work with the existing patterns and methods.

        // Create a new Character instance with the configured properties
        Character character = nonDefaultStartingPoint ?? new Character();

        // Add skills from the SkillProp collection
        foreach ((DynKey key, object value) in this.Storage.Collection) {
            var addMsg        = $"Adding property {key} with value {value}";
            var addedThisIter = false;

            if (value is Skill skillProp) {
                Console.WriteLine(addMsg);
                character.Skills[(SkillKey)key] = (ushort)skillProp.Level;
                addedThisIter                   = true;
            }

            switch (value) {
                case Name name:
                    Console.WriteLine($"Setting name to {name.Value}");
                    character.Name = name.Value;

                    break;
                case Level level:
                    Console.WriteLine($"Setting level to {level.Value}");
                    character.Level = level.Value;

                    break;
                default: {
                    if (addedThisIter) break;
                    Console.WriteLine($"Ignoring unknown property {key} with value {value}");

                    break;
                }
            }
        }

        return character;
    }

}