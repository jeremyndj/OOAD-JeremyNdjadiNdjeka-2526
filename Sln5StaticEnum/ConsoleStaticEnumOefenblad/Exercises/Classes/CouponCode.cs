using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

// 1. Maak in "Exercises/Classes" een klasse "CouponCode":
//   - private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
//   - property "Code" van type string.
//   - property "IsGeldig" van type bool met alleen getter:
//     gebruik Regex.IsMatch(...) om te controleren of Code geldig is
//   - constructor met één parameter
//
// 2. Voeg vervolgens statische methode ControleerCode(string code) toe, die toelaat een gegeven code te controleren
//
// 3. Voeg tenslotte nog een statische methode Beschrijf(string code) toe: 
//   - als de code geldig is, geef je een tekst terug in dit formaat:
//     "Prefix=ABC, Nummer=12, Regio=DE"
//   - als de code ongeldig is, geef je "ongeldige code" terug.
namespace ConsoleStaticEnumOefenblad.Exercises.Classes
{
    internal class CouponCode
    {
        private static string _couponRegex = @"^[A-Z]{3}\d{2}-[A-Z]{2}$";
        public string Code { get; set; }
        public bool IsGeldig 
        { 
            get
            { 
                return Regex.IsMatch(Code, _couponRegex); 
            }
        }
        public CouponCode(string code) 
        {
            Code = code;
        }

        public static bool ControleerCode(string code) 
        { 
            return Regex.IsMatch(code, _couponRegex);
        }
    }
}
