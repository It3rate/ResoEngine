using System;
using System.Collections.Generic;
using System.Text;
using ResoEngine.Support;

namespace ResoEngine
{
    public class Axis : IAlgebraic<Proportion>, IValue, ISubSpace
    {
        // 2 Proportion-level elements (Left, Right)
        public int Dims => 2;
        public Chirality Chirality { get; }
        public Proportion Left { get; } // will always be chirality.Con, imaginary value
        public Proportion Right { get; }// will always be chirality.Pro, rational value



        public long Min => Left.GetNumerator();
        public long Max => Right.GetNumerator();
        public long Unot => Left.GetDenominator();
        public long Unit => Right.GetDenominator();

        public ISpace[] FrameElements => [Left, Right];
        public IValue[] ChildValues => [Left, Right];

        public Axis(Proportion left, Proportion right, Chirality chirality)
        {
            left.ForceChirality(Chirality.Con);
            right.ForceChirality(Chirality.Pro);
            Left = left;
            Right = right;
            Chirality = chirality;

            // Complex number algebra at the Proportion level:
            // index 0 = Left (imaginary), index 1 = Right (real)
            Algebra = new[] {
                new AlgebraEntry(1, 1, 1, +1), // real*real -> +real   (ac)
                new AlgebraEntry(0, 0, 1, -1), // imag*imag -> -real   (i²=-1)
                new AlgebraEntry(1, 0, 0, +1), // real*imag -> +imag   (ad)
                new AlgebraEntry(0, 1, 0, +1), // imag*real -> +imag   (bc)
            };
        }

        public AlgebraEntry[] Algebra { get; }

        /// <summary>
        /// IAlgebraic<Proportion> get Left (0) or Right (1) as Proportion elements.
        /// </summary>
        public Proportion GetElement(int index) => index switch
        {
            0 => Left,
            1 => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        /// <summary>
        /// Multiply two Axes at the Proportion level using complex algebra.
        /// Returns an Axis with the result packed into Left (imaginary) and Right (real).
        /// </summary>
        public static Axis operator *(Axis a, Axis b)
        {
            var result = new Proportion[2];
            for (int i = 0; i < 2; i++) result[i] = Proportion.Zero;
            foreach (var entry in a.Algebra)
            {
                var product = a.GetElement(entry.LeftIndex) * b.GetElement(entry.RightIndex);
                if (entry.Sign < 0) product = -product;
                result[entry.ResultIndex] = result[entry.ResultIndex] + product;
            }
            return FromProportions(result[0], result[1], a.Chirality);
        }

        /// <summary>
        /// Construct an Axis from algebra results (Pro chirality Proportions).
        /// Converts the imaginary Proportion into Con perspective for Left.
        /// </summary>
        public static Axis FromProportions(Proportion imaginary, Proportion real, Chirality chirality)
        {
            // imaginary is in Pro chirality from algebra. Convert to Con for Left:
            // swap num/den so the value is preserved under the Con perspective.
            var left = new Proportion(imaginary.GetDenominator(), imaginary.GetNumerator(), Chirality.Con);
            return new Axis(left, real, chirality);
        }

        /// <summary>
        /// Fold 2D (Left, Right) down to a single Proportion.
        /// Returns the Right (real/rational) component - the "forward" projection.
        /// </summary>
        public Proportion Fold() => Right;

        // Long-level access (4 components) for backward compatibility
        // convert to binary and each 0|1 to left right and walk the structure as a binary tree.
        // merge when you stop, so a Proportion can be merged to N/D (or any other stop point, like an area)
        public int LongDims => 4;
        public long this[int index] => GetValueByIndex(index);
        public long GetValueByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Left.GetDenominator();
                case 1:
                    return Left.GetNumerator();
                case 2:
                    return Right.GetDenominator();
                case 3:
                default:
                    return Right.GetNumerator();
            }
        }
        public double[] GetValues() => [Left.GetValues()[0], Right.GetValues()[0]];
        public long GetTicksByPerspective(Chirality perspective) =>
            perspective.IsPositive() ? Right.GetNumerator() : Left.GetNumerator();

    }
}
