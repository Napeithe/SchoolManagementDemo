namespace SchoolManagement.Infrastructure
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public CustomLanguageManager()
        {
            AddTranslation("pl", "EmailValidator", "Pole nie zawiera poprawnego adresu email.");
            AddTranslation("pl", "GreaterThanOrEqualValidator", "Wartość pola musi być równa lub większa niż '{ComparisonValue}'.");
            AddTranslation("pl", "GreaterThanValidator", "Wartość musi być większa niż '{ComparisonValue}'.");
            AddTranslation("pl", "LengthValidator", "Długość pola musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
            AddTranslation("pl", "MinimumLengthValidator", "Długość pola musi być większa lub równa {MinLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
            AddTranslation("pl", "MaximumLengthValidator", "Długość pola musi być mniejszy lub równy {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
            AddTranslation("pl", "LessThanOrEqualValidator", "Wartość pola musi być równa lub mniejsza niż '{ComparisonValue}'.");
            AddTranslation("pl", "LessThanValidator", "Wartość pola musi być mniejsza niż '{ComparisonValue}'.");
            AddTranslation("pl", "NotEmptyValidator", "Pole nie może być puste.");
            AddTranslation("pl", "NotEqualValidator", "Pole nie może być równe '{ComparisonValue}'.");
            AddTranslation("pl", "NotNullValidator", "Pole nie może być puste.");
            AddTranslation("pl", "PredicateValidator", "Określony warunek nie został spełniony dla pola.");
            AddTranslation("pl", "AsyncPredicateValidator", "Określony warunek nie został spełniony dla pola.");
            AddTranslation("pl", "RegularExpressionValidator", "Niepoprawny format.");
            AddTranslation("pl", "EqualValidator", "Wartość pola musi być równa '{ComparisonValue}'.");
            AddTranslation("pl", "ExactLengthValidator", "Pole musi posiadać długość {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
            AddTranslation("pl", "InclusiveBetweenValidator", "Wartość pola musi się zawierać pomiędzy {From} i {To}. Wprowadzono {Value}.");
            AddTranslation("pl", "ExclusiveBetweenValidator", "Wartość pola musi się zawierać pomiędzy {From} i {To} (wyłącznie). Wprowadzono {Value}.");
            AddTranslation("pl", "CreditCardValidator", "Pole nie zawiera poprawnego numer karty kredytowej.");
            AddTranslation("pl", "ScalePrecisionValidator", "Wartość pola nie może mieć więcej niż {ExpectedPrecision} cyfr z dopuszczalną dokładnością {ExpectedScale} cyfr po przecinku. Znaleziono {Digits} cyfr i {ActualScale} cyfr po przecinku.");
            AddTranslation("pl", "EmptyValidator", "Pole musi być puste.");
            AddTranslation("pl", "NullValidator", "Pole musi być puste.");
            AddTranslation("pl", "EnumValidator", "Pole ma zakres wartości, który nie obejmuje {PropertyValue}.");
            AddTranslation("pl", "Length_Simple", "Długość pola musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów).");
            AddTranslation("pl", "MinimumLength_Simple", "Długość pola musi być większa lub równa {MinLength} znaki(ów).");
            AddTranslation("pl", "MaximumLength_Simple", "Długość pola musi być mniejszy lub równy {MaxLength} znaki(ów).");
            AddTranslation("pl", "ExactLength_Simple", "Pole musi posiadać długość {MaxLength} znaki(ów).");
            AddTranslation("pl", "InclusiveBetween_Simple", "Długość pola musi być mniejszy lub równy {MaxLength} znaki(ów).");
            AddTranslation("pl", "MaximumLength_Simple", "Wartość pola musi się zawierać pomiędzy {From} i {To}.");
        }
    }
}
