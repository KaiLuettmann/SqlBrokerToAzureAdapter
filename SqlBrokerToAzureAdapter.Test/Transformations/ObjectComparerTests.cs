using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using FluentAssertions.Execution;
using Force.DeepCloner;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Test.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Transformations;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Test.Transformations
{
    public class StringObjectComparerTests : ObjectComparerTests<string>
    {
        public StringObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class IntegerObjectComparerTests : ObjectComparerTests<int>
    {
        public IntegerObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class GuidObjectComparerTests : ObjectComparerTests<Guid>
    {
        public GuidObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class DateTimeObjectComparerTests : ObjectComparerTests<DateTime>
    {
        public DateTimeObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class BooleanObjectComparerTests : ObjectComparerTests<bool>
    {
        public BooleanObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class ByteObjectComparerTests : ObjectComparerTests<byte>
    {
        public ByteObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class SbyteObjectComparerTests : ObjectComparerTests<sbyte>
    {
        public SbyteObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class CharObjectComparerTests : ObjectComparerTests<char>
    {
        public CharObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class DecimalObjectComparerTests : ObjectComparerTests<decimal>
    {
        public DecimalObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class DoubleObjectComparerTests : ObjectComparerTests<double>
    {
        public DoubleObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class FloatObjectComparerTests : ObjectComparerTests<float>
    {
        public FloatObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class UintObjectComparerTests : ObjectComparerTests<uint>
    {
        public UintObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class LongObjectComparerTests : ObjectComparerTests<long>
    {
        public LongObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class UlongObjectComparerTests : ObjectComparerTests<ulong>
    {
        public UlongObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class ShortObjectComparerTests : ObjectComparerTests<short>
    {
        public ShortObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public class UshortObjectComparerTests : ObjectComparerTests<ushort>
    {
        public UshortObjectComparerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
    }

    public abstract class ObjectComparerTests<T>
    {
        private readonly Fixture<T> _fixture;

        protected ObjectComparerTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture<T>(testOutputHelper.BuildLoggerFor<ObjectComparer<FakeObject<T>>>());
        }

        [Fact]
        public void Compare_WithOneUpdatedPairWhereOldValueIsEqualToNewValue_ResultShouldHaveNoDifferences()
        {
            //Arrange
            _fixture.SetupOneUpdatedPairWhereOldValueIsEqualToNewValue();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = testObject.Compare(_fixture.UpdatedPairs).ToList();

            //Assert
            results.Should().HaveCount(1);
            var result = results.Single();
            result.IsEqual.Should().BeTrue();
            result.Differences.Should().BeEmpty();
        }

        [Fact]
        public void Compare_WithOneUpdatedPairWhereProperty1IsEqual_DifferenceShouldBeProperty2()
        {
            //Arrange
            _fixture.SetupOneUpdatedPairWherePropertiesAreEqual(x => x.Property1);
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = testObject.Compare(_fixture.UpdatedPairs).ToList();

            //Assert
            using var _ = new AssertionScope();
            results.Should().HaveCount(1);
            var result = results.Single();
            result.IsEqual.Should().BeFalse();
            result.Differences.Should().HaveCount(1);
            var difference = result.Differences.Single();
            difference.MemberPath.Should().Be(nameof(FakeObject<string>.Property2));
            result.UpdatedPair.OldValue.Property2.Should().NotBe(result.UpdatedPair.NewValue.Property2);
        }

        [Fact]
        public void Compare_WithOneUpdatedPairWhereProperty1IsEqual_DifferenceShouldNotBeProperty1()
        {
            //Arrange
            _fixture.SetupOneUpdatedPairWherePropertiesAreEqual(x => x.Property1);
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = testObject.Compare(_fixture.UpdatedPairs).ToList();

            //Assert
            using var _ = new AssertionScope();
            results.Should().HaveCount(1);
            var result = results.Single();
            result.IsEqual.Should().BeFalse();
            result.Differences.Should().HaveCount(1);
            var difference = result.Differences.Single();
            difference.MemberPath.Should().NotBe(nameof(FakeObject<string>.Property1));
            result.UpdatedPair.OldValue.Property1.Should().Be(result.UpdatedPair.NewValue.Property1);
        }


        [Fact]
        public void Compare_WithTwoUpdatedPairsWhereOneObjectIsEqual_DifferenceShouldHaveCountOfOne()
        {
            _fixture.SetupTwoUpdatedPairsWhereOneObjectIsEqual(x => x.Property1);
            var testObject = _fixture.CreateTestObject();

            var results = testObject.Compare(_fixture.UpdatedPairs).ToList();

            using var _ = new AssertionScope();
            results.Should().HaveCount(2);
            var differences = results.SelectMany(x => x.Differences).ToList();
            differences.Should().NotBeNull();
            differences.Should().HaveCount(1);
            var difference = differences.Single();
            difference.MemberPath.Should().NotBe(nameof(FakeObject<string>.Property1));
        }

        [Fact]
        public void Compare_WithTwoUpdatedPairsWhereOneObjectIsEqual_ShouldHaveOneEqualCountOfOneResult()
        {
            _fixture.SetupTwoUpdatedPairsWhereOneObjectIsEqual(x => x.Property1);
            var testObject = _fixture.CreateTestObject();

            var results = testObject.Compare(_fixture.UpdatedPairs).ToList();

            using var _ = new AssertionScope();
            var equalResults = results.Where(x => x.IsEqual).ToList();
            equalResults.Should().HaveCount(1);
            var equalResult = equalResults.Single();
            equalResult.Differences.Should().BeEmpty();
            equalResult.UpdatedPair.NewValue.Should().BeEquivalentTo(equalResult.UpdatedPair.OldValue);
        }

        private class Fixture<TFakeObjectPropertyType>
        {
            private readonly ILogger<ObjectComparer<FakeObject<TFakeObjectPropertyType>>> _logger;

            public Fixture(ILogger<ObjectComparer<FakeObject<TFakeObjectPropertyType>>> logger)
            {
                _logger = logger;
            }

            public List<UpdatedPair<FakeObject<TFakeObjectPropertyType>>> UpdatedPairs { get; private set; }

            public ObjectComparer<FakeObject<TFakeObjectPropertyType>> CreateTestObject()
            {
                return new ObjectComparer<FakeObject<TFakeObjectPropertyType>>(_logger);
            }

            private void AddOneUpdatedPairWherePropertiesAreEqual(params Expression<Func<FakeObject<TFakeObjectPropertyType>, TFakeObjectPropertyType>>[] equalProperties)
            {
                var builder = new Fixture();
                //ensure number are unique
                builder.Customizations.Add(new RandomNumericSequenceGenerator(240, byte.MaxValue, int.MaxValue));


                var oldValue = builder.Create<FakeObject<TFakeObjectPropertyType>>();

                IPostprocessComposer<FakeObject<TFakeObjectPropertyType>> composer = builder.Build<FakeObject<TFakeObjectPropertyType>>();
                foreach (var equalProperty in equalProperties)
                {
                    var equalPropertyValue = equalProperty.Compile().Invoke(oldValue);
                    composer = builder.Build<FakeObject<TFakeObjectPropertyType>>().With(equalProperty, equalPropertyValue);
                }

                var newValue = composer.Create();

                UpdatedPairs.Add(
                    new UpdatedPair<FakeObject<TFakeObjectPropertyType>>
                    {
                        OldValue = oldValue,
                        NewValue = newValue
                    });
            }

            private void AddOneUpdatedPairWhereOldValueIsEqualToNewValue()
            {
                var oldValue = new Fixture().Create<FakeObject<TFakeObjectPropertyType>>();
                UpdatedPairs.Add(
                    new UpdatedPair<FakeObject<TFakeObjectPropertyType>>
                    {
                        OldValue = oldValue,
                        NewValue = oldValue.DeepClone()
                    });
            }

            private void InitializeUpdatedPairs()
            {
                UpdatedPairs = new List<UpdatedPair<FakeObject<TFakeObjectPropertyType>>>();
            }

            public void SetupOneUpdatedPairWhereOldValueIsEqualToNewValue()
            {
                InitializeUpdatedPairs();

                AddOneUpdatedPairWhereOldValueIsEqualToNewValue();
            }

            public void SetupOneUpdatedPairWherePropertiesAreEqual(params Expression<Func<FakeObject<TFakeObjectPropertyType>, TFakeObjectPropertyType>>[] equalProperties)
            {
                InitializeUpdatedPairs();

                AddOneUpdatedPairWherePropertiesAreEqual(equalProperties);
            }

            public void SetupTwoUpdatedPairsWhereOneObjectIsEqual(params Expression<Func<FakeObject<TFakeObjectPropertyType>, TFakeObjectPropertyType>>[] equalProperties)
            {
                InitializeUpdatedPairs();
                AddOneUpdatedPairWherePropertiesAreEqual(equalProperties);
                AddOneUpdatedPairWhereOldValueIsEqualToNewValue();
            }
        }

        private class FakeObject<TPropertyValue>
        {
            public TPropertyValue Property1 { get; set; }
            public TPropertyValue Property2 { get; set; }
        }
    }
}