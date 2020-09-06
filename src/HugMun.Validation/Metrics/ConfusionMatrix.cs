using System;
using System.Collections.Generic;
using System.Linq;

namespace HugMun.Validation
{
    public sealed class ConfusionMatrix<TSolution>
    {
        private readonly SortedDictionary<TSolution, SortedDictionary<TSolution, int>> matrix;

        public IReadOnlyList<TSolution> Labels => matrix.Keys.ToList().AsReadOnly(); 

        internal ConfusionMatrix(SortedDictionary<TSolution, SortedDictionary<TSolution, int>> matrix)
        {
            this.matrix = matrix;
        }

        public IReadOnlyList<int> this[int row]
        {
            get
            {
                if (row < 0 || row >= matrix.Count) throw new ArgumentOutOfRangeException(nameof(row));
                return matrix.ElementAt(row).Value.Values.ToList().AsReadOnly();
            }
        }

        public int this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= matrix.Count) throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 0 || column >= matrix.Count) throw new ArgumentOutOfRangeException(nameof(row));
                return matrix.ElementAt(row).Value.ElementAt(column).Value;
            }
        }

        public IReadOnlyDictionary<TSolution, int> this[TSolution row] => matrix[row];

        public int this[TSolution row, TSolution column] => matrix[row][column];
    }
}
