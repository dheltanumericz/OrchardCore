using System.Collections.Generic;
using System.Linq;
using OrchardCore.Localization;
using Xunit;

namespace OrchardCore.Tests.Localization
{
    public class CultureDictionaryTests
    {
        private static PluralizationRuleDelegate _arPluralRule = n => (n == 0 ? 0 : n == 1 ? 1 : n == 2 ? 2 : n % 100 >= 3 && n % 100 <= 10 ? 3 : n % 100 >= 11 ? 4 : 5);
        private static PluralizationRuleDelegate _csPluralRule = n => ((n == 1) ? 0 : (n >= 2 && n <= 4) ? 1 : 2);

        [Fact]
        public void MergeAddsRecordToEmptyDictionary()
        {
            var dictionary = new CultureDictionary("cs", _csPluralRule);
            var record = new CultureDictionaryRecord("ball", null, new[] { "míč", "míče", "míčů" });

            dictionary.MergeTranslations(new[] { record });

            Assert.Equal(dictionary.Translations[record.Key], record.Translations);
        }

        [Fact]
        public void MergeOverwritesTranslationsForSameKeys()
        {
            var dictionary = new CultureDictionary("cs", _csPluralRule);
            var record = new CultureDictionaryRecord("ball", null, new[] { "míč", "míče", "míčů" });
            var record2 = new CultureDictionaryRecord("ball", null, new[] { "balón", "balóny", "balónů" });

            dictionary.MergeTranslations(new[] { record });
            dictionary.MergeTranslations(new[] { record2 });

            Assert.Equal(dictionary.Translations[record.Key], record2.Translations);
        }

        [Fact]
        public void IndexerReturnNullIfKeyDoesntExist()
        {
            var dictionary = new CultureDictionary("cs", _csPluralRule);

            var translation = dictionary["ball"];

            Assert.Null(translation);
        }

        [Fact]
        public void IntexerThrowsPluralFormNotFoundExceptionIfSpecifiedPluralFormDoesntExist()
        {
            var dictionary = new CultureDictionary("cs", _csPluralRule);
            var record = new CultureDictionaryRecord("ball", null, new[] { "míč", "míče" });
            dictionary.MergeTranslations(new[] { record });

            Assert.Throws<PluralFormNotFoundException>(() => dictionary["ball", 5]);
        }

        [Fact]
        public void EnumerateCultureDictionary()
        {
            // Arrange
            var dictionary = new CultureDictionary("ar", _arPluralRule);
            dictionary.MergeTranslations(new List<CultureDictionaryRecord>
            {
                new CultureDictionaryRecord("Hello", null, new[] { "مرحبا" }),
                new CultureDictionaryRecord("Bye", null, new[] { "مع السلامة" })
            });

            // Act & Assert
            Assert.NotEmpty(dictionary);

            foreach (var record in dictionary)
            {
                Assert.NotNull(record.Key);
                Assert.Single(record.Translations);
            }

            Assert.Equal(2, dictionary.Count());
        }
    }
}
