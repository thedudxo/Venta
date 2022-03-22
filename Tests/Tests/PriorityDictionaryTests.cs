using NUnit.Framework;
using DudCo.Events;

namespace Tests.PriorityDictionaries
{
    public class PriorityDictionaryTests
    {
        class TestPriorityDictionary : PriorityDictionary { }

        PriorityDictionary priorites = new TestPriorityDictionary();

        [SetUp]
        public void Setup()
        {
            priorites = new TestPriorityDictionary();
        }

        class itemA { }
        class itemB { }
        class itemC { }

        class Add : PriorityDictionaryTests
        {
            [Test]
            public void Contains_AdddedItemGeneric()
            {
                priorites.Add<itemA>(0);

                Assert.That(priorites.ContainsKey(typeof(itemA)));
            }

            [Test]
            public void Contains_AdddedItemGeneric_CorrectPriority()
            {
                const int priority = 1;

                priorites.Add<itemA>(priority);

                Assume.That(priorites.ContainsKey(typeof(itemA)));
                Assert.AreEqual(priority, priorites[typeof(itemA)]);
            }

            [Test]
            public void Contains_AdddedItemType()
            {
                priorites.Add(typeof(itemA), 0);

                Assert.That(priorites.ContainsKey(typeof(itemA)));
            }

            [Test]
            public void Contains_AdddedItemType_CorrectPriority()
            {
                const int priority = 1;

                priorites.Add(typeof(itemA), priority);

                Assume.That(priorites.ContainsKey(typeof(itemA)));
                Assert.AreEqual(priority, priorites[typeof(itemA)]);
            }

            [Test]
            public void AddExistingItem_ThrowsArgumentException()
            {
                priorites.Add<itemA>(0);

                Assert.Throws<System.ArgumentException>(
                    () => priorites.Add<itemA>(0)
                    );
            }
        }

        class PriorityOf : PriorityDictionaryTests
        {
            [Test]
            public void PriorityOfGeneric_AddedItem_Correct()
            {
                const int priority = 1;

                priorites.Add<itemA>(priority);

                Assert.AreEqual(priority, priorites.PriorityOf<itemA>());
            }

            [Test]
            public void PriorityOfType_AddedItem_Correct()
            {
                const int priority = 1;

                priorites.Add<itemA>(priority);

                Assert.AreEqual(priority, priorites.PriorityOf(typeof(itemA)));
            }

            [Test]
            public void PriortiyOfType_NotAddeditem_ThrowsKeyNotFoundExceptionException()
            {
                Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                    () => priorites.PriorityOf(typeof(itemA)) 
                    );
            }


            [Test]
            public void PriortiyOfGeneric_NotAddeditem_ThrowsKeyNotFoundExceptionException()
            {
                Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                    () => priorites.PriorityOf<itemA>()
                    );
            }
        }

        class Indexer : PriorityDictionaryTests
        {
            [Test]
            public void TypeIndexerOf_AddedItem_Correct()
            {
                const int priority = 5;

                priorites.Add<itemA>(priority);

                Assert.AreEqual(priority, priorites[typeof(itemA)]);
            }

            [Test]
            public void TypeIndexerOf_NotAddedItem_ThrowsKeyNotFoundExceptionException()
            {
                Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                () => _= priorites[typeof(itemA)]
                );
            }
        }

        class AddAfter : PriorityDictionary
        {
            [Test]
            public void AddAfter_PreviousItem_HasLowerPriority()
            {
                Add<itemA>(0);

                Add<itemB>(After<itemA>());

                Assert.Less(PriorityOf<itemB>(), PriorityOf<itemA>());
            }

            [Test]
            public void AddAfter_NotFoundItem_ThrowsKeyNotFoundExceptionException()
            {
                Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                    () => Add<itemB>(After<itemA>())
                    );
            }
        }

        class AddBefore : PriorityDictionary
        {
            [Test]
            public void AddBefore_PreviousItem_HasGreaterPriority()
            {
                Add<itemA>(0);

                Add<itemB>(Before<itemA>());

                Assert.Greater(PriorityOf<itemB>(), PriorityOf<itemA>());
            }

            [Test]
            public void AddBefore_NotFoundItem_ThrowsKeyNotFoundExceptionException()
            {
                Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
                    () => Add<itemB>(Before<itemA>())
                    );
            }
        }
    }
}