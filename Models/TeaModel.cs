using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SQLite;

namespace com.mahonkin.tim.TeaDataService.DataModel
{
    /// <summary>
    /// Class that defines the tea variety.
    /// </summary>
    [Table("TeaVarieties")]
    public class TeaModel
    {
        #region Private Fields
        private int _id;
        private int _brewTemp;
        private string _name = string.Empty;
        private TimeSpan _steepTime;
        #endregion Private Fields      

        #region Public Properties
        /// <summary>
        /// The database-assigned unique ID for this tea record. There should be no reason to set it in code.
        /// </summary>
        [Column("ID"), PrimaryKey, AutoIncrement]
        [JsonRequired]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// The name of the tea variety. Could be a traditional name or a commercial name.
        /// <br>
        /// Essentially this is whatever the user will identify the tea variety by. For example, 'Earl Grey' or 'SleepyTime'.
        /// </br>
        /// </summary>
        [Required(ErrorMessage = "Tea must have a name."), MinLength(1, ErrorMessage = "Tea Must have a name.")]
        [Column("Name"), Unique, NotNull]
        [JsonRequired]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The amount of time to allow the tea to steep in the mug or cup.
        /// <br>If the user tries to set the steep time to a value of 30 minutes or longer an ArgumentException will be thrown.</br>
        /// </summary>
        [Required(ErrorMessage = "Tea must have a steep time."), MinLength(1, ErrorMessage = "Tea must have a steep time.")]
        [DisplayFormat(DataFormatString = @"hh\:mm\:ss")]
        [Column("Steeptime"), NotNull]
        [JsonRequired]
        public TimeSpan SteepTime
        {
            get => _steepTime;
            set => _steepTime = value;
        }

        /// <summary>
        /// The water temperature in farenheit degrees at which to steep the tea.
        /// <br>If the user attempts to set a temperature value greater than boiling (212 degrees farenheit) the value will be set to 212.</br>
        /// </summary>
        [Required(ErrorMessage = "Tea must have a Brew Temperature.")]
        [Range(1, 212, ErrorMessage = "Brew temerature must be between 1 and 212")]
        [Column("Brewtemp"), NotNull]
        [JsonRequired]
        public int BrewTemp
        {
            get => _brewTemp;
            set => _brewTemp = value;
        }
        #endregion Public Properties

        #region Constructors
        /// <summary>
        /// Do not use in your code.
        /// <br>Public, parameterless constructor required by the SQLite serialization/deserialization routines. If you try to use it you will almost
        /// undoubtedly get an exception.</br>
        /// </summary>
        public TeaModel() : this(string.Empty)
        {
        }

        /// <summary>
        /// Creates a new Tea Variety object.
        /// <br>The user must specify a value for variety name and may specify optional values for steep time and brew temperature with these limitations:</br>
        /// <list type="bullet">
        /// <item>Steep time must be provided as a string in standard 'mm:ss' format or an ArgumentException will be thrown.</item>
        /// <item>Providing a steep time of greater than 30 minutes will throw an ArgumentException.</item>
        /// <item>Providing a brew temperature of greater than boiling (212 degrees farenheit) will result in the temperature being set to boiling.</item>
        /// <item>Not providing a step time will result in a steep time of 2 minutes.</item>
        /// <item>Not providing a brew temperature will result in the brew temperature being set to boiling (212 degrees farenheit).</item>
        /// </list>
        /// </summary>
        /// <param name="name">The user-identified name of the tea variety. May be a common name like 'Earl Grey' or a commercial name like 'SleepyTime'.</param>
        /// <param name="steepTime">The amount of time, expressed as a string in standard 'mm:ss' format, the tea should steep. If set to a time greater than 30 minutes an ArgumentException is thrown.</param>
        /// <param name="brewTemp">The temperature in degrees farenheit at which the tea should steep. If attempting to set to a value greater than 212 (boiling) it will be set to 212.</param>
        /// <exception cref="ArgumentException">ArgumentException</exception>
        public TeaModel(string name, string steepTime = "00:02:00", int brewTemp = 212)
        {
            if (TimeSpan.TryParse(steepTime, out TimeSpan time) == false)
            {
                throw new ArgumentException($"Could not parse provided steep time {steepTime}", nameof(steepTime));
            }
            SteepTime = (time <= TimeSpan.FromMinutes(30) && time > TimeSpan.Zero) ? time : throw new ArgumentException($"Steep times of greater than 30 minutes ({steepTime}) really don't make sense.", nameof(steepTime));
            Name = name;
            BrewTemp = (brewTemp <= 212 && brewTemp > 0) ? brewTemp : 212;
        }

        /// <summary>
        /// Creates a new Tea Variety object.
        /// <br>The user must specify values for variety name and steep time and may specify an optional brew temperature with these limitations:</br>
        /// <list type="bullet">
        /// <item>Providing a steep time of greater than 30 minutes will throw an ArgumentException.</item>
        /// <item>Providing a brew temperature of greater than boiling (212 degrees farenheit) will result in the temperature being set to boiling.</item>
        /// <item>Not providing a brew temperature will result in the brew temperature being set to boiling (212 degrees farenheit).</item>
        /// </list>
        /// </summary>
        /// <param name="name">The user-identified name of the tea variety. May be a common name like 'Earl Grey' or a commercial name like 'SleepyTime'.</param>
        /// <param name="steepTime">The amount of time, expressed as a TimeSpan object, the tea should steep. If set to a time greater than 30 minutes an ArgumentException is thrown.</param>
        /// <param name="brewTemp">The temperature in degrees farenheit at which the tea should steep. If attempting to set to a value greater than 212 (boiling) it will be set to 212.</param>
        /// <exception cref="ArgumentException">ArgumentException</exception>
        public TeaModel(string name, TimeSpan steepTime, int brewTemp = 212)
        {
            Name = name;
            SteepTime = (steepTime <= TimeSpan.FromMinutes(30)) ? steepTime : throw new ArgumentException($"Steep times greater than 30 minutes ({steepTime}) don't really make sense.", nameof(steepTime));
            BrewTemp = (brewTemp <= 212 && brewTemp > 32) ? brewTemp : 212;
        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// Validates the tea's data.
        /// </summary>
        /// <param name="tea"></param>
        /// <returns>The tea with valid properties if it could be validated.
        /// An exception otherwise.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static TeaModel ValidateTea(TeaModel tea)
        {
            tea.Name = tea.Name.Trim(); // Clean up preceeding and trailing whitespace.
            if (string.IsNullOrWhiteSpace(tea.Name))
            {
                throw new ArgumentNullException(nameof(tea.Name), "Tea variety must have a name.");
            }
            if (tea.BrewTemp > 212 || tea.BrewTemp <= 32)
            {
                tea.BrewTemp = 212; 
            }
            if (tea.SteepTime > TimeSpan.FromMinutes(30) || tea.SteepTime < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException(nameof(tea.SteepTime), "Steep Time must be more than zero seconds and less than 30 minutes.");
            }
            return tea;
        }
        #endregion Public Methods
    }


    /// <summary>
    /// Potentially helpful extension methods on the TeaModel class.
    /// </summary>
    public static class TeaModelExtensions
    {
        /// <summary>
        /// <inheritdoc cref="TeaModel.ValidateTea(TeaModel)"/>
        /// </summary>
        public static TeaModel Validate(this TeaModel tea)
        {
            return TeaModel.ValidateTea(tea);
        }
    }
}
