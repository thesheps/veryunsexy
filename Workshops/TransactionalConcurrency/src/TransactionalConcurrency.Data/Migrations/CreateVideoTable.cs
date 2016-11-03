using System;
using System.Collections.Generic;
using FluentMigrator;

namespace TransactionalConcurrency.Data.Migrations
{
    [Migration(1)]
    public class CreateVideoTable : Migration
    {
        public override void Up()
        {
            Create.Table("Video")
                .WithColumn("VideoId").AsInt32().PrimaryKey("PK_VideoId").Identity()
                .WithColumn("Title").AsString()
                .WithColumn("SaleIncVat").AsDecimal();

            for (var i = 0; i < 100; i++)
            {
                Insert.IntoTable("Video")
                    .Row(CreateRow(_titles[_random.Next(_titles.Count - 1)], 100));
            }
        }

        private static object CreateRow(string title, decimal saleIncVat)
        {
            return new
            {
                SaleIncVat = saleIncVat,
                Title = title
            };
        }

        public override void Down()
        {
            Delete.Table("Video");
        }

        #region Data

        private readonly Random _random = new Random();

        private readonly IList<string> _titles = new List<string>
        {
            "Brief Encounter",
            "Casablanca",
            "Before Sunrise",
            "Before Sunset",
            "Breathless",
            "In the Mood for Love",
            "The Apartment",
            "Hannah & Her Sisters",
            "Eternal Sunshine of the Spotless Mind",
            "Room With a View",
            "Jules et Jim",
            "All That Heaven Allows",
            "Gone with the Wind",
            "An Affair to Remember",
            "Umbrellas of Cherbourg",
            "Lost in Translation",
            "Roman Holiday",
            "Wall-E",
            "My Night With Maud",
            "Voyage to Italy",
            "Dr Zhivago",
            "Harold & Maude",
            "When Harry Met Sally",
            "Say Anything....",
            "Fabulous Baker Boys",
            "A Matter of Life & Death",
            "Chinatown",
            "Touch of Evil",
            "Vertigo",
            "Badlands",
            "Rashomon",
            "Double Indemnity",
            "Get Carter",
            "Pulp Fiction",
            "Hidden",
            "Goodfellas",
            "The Conversation",
            "Bonnie & Clyde",
            "The Killing",
            "French Connection",
            "The Big Sleep",
            "La Ceremonie",
            "Point Blank",
            "Hard Boiled",
            "Long Good Friday",
            "A Prophet",
            "Heat",
            "Scarface (1983)",
            "Miller’s Crossing",
            "Postman Always Rings Twice  (1942)",
            "Jour Se Leve",
            "Annie Hall",
            "Borat",
            "Some Like it Hot",
            "Team America",
            "Dr Strangelove",
            "The Ladykillers",
            "Duck Soup",
            "Rushmore",
            "Kind Hearts & Coronets",
            "Monty Python’s Life of Brian",
            "Airplane!",
            "Election",
            "His Girl Friday",
            "The Big Lebowski",
            "This Is Spinal Tap",
            "Bringing Up Baby",
            "There’s Something About Mary",
            "Dazed and Confused",
            "MASH",
            "Groundhog Day",
            "Clueless",
            "The Great Dictator",
            "Clerks",
            "The Jerk",
            "Shaun of the Dead",
            "Apocalypse Now",
            "North by Northwest",
            "Once Upon a Time in the West",
            "The Wild Bunch",
            "Deliverance",
            "City of God",
            "Paths of Glory",
            "The Wages of Fear",
            "Crouching Tiger Hidden Dragon",
            "The Thin Red Line",
            "Raiders of the Lost Ark",
            "Bullitt",
            "Ran",
            "Die Hard",
            "The Adventures of Robin Hood",
            "The Searchers",
            "Goldfinger",
            "Full Metal Jacket",
            "Last of the Mohicans",
            "Deer Hunter",
            "Gladiator",
            "Rome Open City",
            "Butch Cassidy",
            "Where Eagles Dare",
            "The Incredibles"
        };

        #endregion
    }
}