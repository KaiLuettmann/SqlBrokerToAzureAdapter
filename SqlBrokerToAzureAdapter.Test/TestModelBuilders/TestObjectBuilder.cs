using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using SqlBrokerToAzureAdapter.Test.TestUtilities.AutoFixture;

namespace SqlBrokerToAzureAdapter.Test.TestModelBuilders
{
    internal abstract class TestObjectBuilder<T>
    {
        private readonly Fixture _fixture;
        private readonly List<Func<IPostprocessComposer<T>, IPostprocessComposer<T>>> _transformations =
            new List<Func<IPostprocessComposer<T>, IPostprocessComposer<T>>>();
        protected TestObjectBuilder()
        {
            _fixture = new Fixture();
        }

        public T Create()
        {
            _fixture.Customize<T>(ApplyTransformations);
            return _fixture.Create<T>();
        }

        public IEnumerable<T> CreateMany(int count)
        {
            _fixture.Customize<T>(ApplyTransformations);
            return _fixture.CreateMany<T>(count);
        }

        protected void WithConstructorArgumentFor<TValue>(string paramName, TValue value)
        {
            _fixture
                .ConstructorArgumentFor<T, TValue>(paramName, value);
        }

        protected void With<TProperty>(
            Expression<Func<T, TProperty>> propertyPicker,
            TProperty value)
        {
            AddTransformation(c => c.With(propertyPicker, value));
        }
        
        private void AddTransformation(Func<IPostprocessComposer<T>, IPostprocessComposer<T>> transformation)
        {
            _transformations.Add(transformation);
        }

        private ISpecimenBuilder ApplyTransformations(IPostprocessComposer<T> composer)
        {
            return _transformations.Aggregate(composer, (current, transformation) => transformation(current));
        }
    }
}