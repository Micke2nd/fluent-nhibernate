using System.Linq;
using FluentNHibernate.MappingModel;
using FluentNHibernate.MappingModel.Collections;
using FluentNHibernate.Testing.DomainModel.Mapping;
using NUnit.Framework;

namespace FluentNHibernate.Testing.FluentInterfaceTests
{
    [TestFixture]
    public class OneToManySubPartModelGenerationTests : BaseModelFixture
    {
        [Test]
        public void ComponentShouldSetCompositeElement()
        {
            var mapping = MappingFor<OneToManyTarget>(class_map =>
                class_map.HasMany(x => x.BagOfChildren)
                    .Component(c => c.Map(x => x.Name)));

            mapping.Collections.Single()
                .CompositeElement.ShouldNotBeNull();
        }

        [Test]
        public void ListShouldSetIndex()
        {
            OneToMany(x => x.ListOfChildren)
                .Mapping(m => m.AsList(x =>
                {
                    x.Column("index-column");
                    x.Type<int>();
                }))
                .ModelShouldMatch(x =>
                {
                    x.Index.ShouldNotBeNull();
                    x.Index.Columns.Count().ShouldEqual(1);
                    ((IndexMapping)x.Index).Type.ShouldEqual(new TypeReference(typeof(int)));
                });
        }

        [Test]
        public void ShouldSetElement()
        {
            var mapping = MappingFor<OneToManyTarget>(class_map =>
                    class_map.HasMany(x => x.ListOfChildren)
                        .Element("element"));
            var collection = mapping.Collections.Single();

            collection.Element.ShouldNotBeNull();
            collection.Element.Columns.Count().ShouldEqual(1);
            collection.Element.Type.ShouldEqual(new TypeReference(typeof(ChildObject)));
        }

        [Test]
        public void ElementMappingShouldntHaveOneToMany()
        {
            var mapping = MappingFor<OneToManyTarget>(class_map =>
                class_map.HasMany(x => x.ListOfChildren)
                    .Element("element"));

            mapping.Collections.Single()
                .Relationship.ShouldBeNull();
        }

        [Test]
        public void ShouldPerformKeyColumnMapping()
        {
            var mapping = MappingFor<OneToManyTarget>(class_map =>
                class_map.HasMany(x => x.ListOfChildren)
                    .Key(ke => ke.Columns.Add("col1", c => c.Length(50).Not.Nullable())));
            var column = mapping.Collections.Single().Key.Columns.Single();

            column.Name.ShouldEqual("col1");
            column.Length.ShouldEqual(50);
            column.NotNull.ShouldBeTrue();
        }
    }
}