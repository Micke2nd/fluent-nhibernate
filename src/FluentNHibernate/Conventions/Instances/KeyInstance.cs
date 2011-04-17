using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.MappingModel;

namespace FluentNHibernate.Conventions.Instances
{
    public class KeyInstance : KeyInspector, IKeyInstance
    {
        private readonly KeyMapping mapping;

        public KeyInstance(KeyMapping mapping)
            : base(mapping)
        {
            this.mapping = mapping;
        }

        public void Column(string columnName)
        {
            if (mapping.Columns.UserDefined.Count() > 0)
                return;

            var originalColumn = mapping.Columns.FirstOrDefault();
            var column = originalColumn == null ? new ColumnMapping() : originalColumn.Clone();

            column.Set(x => x.Name, Layer.Conventions, columnName);

            mapping.ClearColumns();
            mapping.AddColumn(column);
        }

        public new void ForeignKey(string constraint)
        {
            mapping.Set(x => x.ForeignKey, Layer.Conventions, constraint);
        }

        public new void PropertyRef(string property)
        {
            mapping.Set(x => x.PropertyRef, Layer.Conventions, property);
        }

        public new IEnumerable<IColumnInstance> Columns
        {
            get
            {
                return mapping.Columns.UserDefined
                    .Select(x => new ColumnInstance(mapping.ContainingEntityType, x))
                    .Cast<IColumnInstance>();
            }
        }
    }
}