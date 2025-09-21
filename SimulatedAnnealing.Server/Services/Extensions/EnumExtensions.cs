using System.ComponentModel;

namespace SimulatedAnnealing.Server.Services.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Pobiera wartość atrybutu DescriptionAttribute dla wartości wyliczeniowej.
    /// Jeśli atrybut nie istnieje, zwraca nazwę wartości jako string.
    /// </summary>
    /// <param name="value">Wartość wyliczeniowa, dla której chcemy pobrać opis.</param>
    /// <returns>Opis z atrybutu lub nazwa wartości wyliczeniowej.</returns>
    public static string GetDescription(this Enum value)
    {
        // Pobieramy typ wyliczenia
        var enumType = value.GetType();

        // Pobieramy informację o polu (wartości) wyliczenia
        var fieldInfo = enumType.GetField(value.ToString());

        if (fieldInfo == null)
        {
            return value.ToString();
        }

        // Pobieramy atrybuty DescriptionAttribute dla danego pola
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        // Jeśli atrybut istnieje, zwracamy jego wartość
        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }

        // W przeciwnym razie, zwracamy nazwę wartości jako string
        return value.ToString();
    }
}
