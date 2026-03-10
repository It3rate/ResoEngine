using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResoEngine.Support;

namespace ResoEngine
{
    public class Axis : IAlgebraic<Proportion>, IValue, ISubSpace
    {
        // --- Frame definition ---

        // 2 Proportion-level elements (Left, Right)
        public int Dims => 2;
        public Chirality Chirality { get; }
        public Proportion Left { get; } // will always be chirality.Con, imaginary value
        public Proportion Right { get; }// will always be chirality.Pro, rational value

        public long Min => Left.GetNumerator();
        public long Max => Right.GetNumerator();
        public long Unot => Left.GetDenominator();
        public long Unit => Right.GetDenominator();

        // --- Injectable algebra ---

        /// <summary>
        /// Complex number algebra: (a+bi)(c+di) = (ac-bd) + (ad+bc)i
        /// Default algebra for Axis when none is specified.
        /// </summary>
        public static readonly AlgebraEntry[] ComplexAlgebra =
        [
            new AlgebraEntry(1, 1, 1, +1), // real*real -> +real   (ac)
            new AlgebraEntry(0, 0, 1, -1), // imag*imag -> -real   (i²=-1)
            new AlgebraEntry(1, 0, 0, +1), // real*imag -> +imag   (ad)
            new AlgebraEntry(0, 1, 0, +1), // imag*real -> +imag   (bc)
        ];

        public AlgebraEntry[] Algebra { get; }

        // --- Child storage (fractal containment) ---

        private readonly List<Axis> _children = new();

        /// <summary>
        /// Children living inside this frame. Each child inherits context (resolution, algebra)
        /// from this parent unless it overrides.
        /// </summary>
        public IReadOnlyList<Axis> Children => _children;

        /// <summary>
        /// Parent frame reference. Null for root frames.
        /// Children can use this for context inheritance (resolution, bounds).
        /// </summary>
        public Axis? Parent { get; private set; }

        // ISubSpace: children are the frame elements and child values
        public ISpace[] FrameElements => _children.ToArray<ISpace>();
        public IValue[] ChildValues => _children.ToArray<IValue>();

        // --- Constructors ---

        public Axis(Proportion left, Proportion right, Chirality chirality, AlgebraEntry[]? algebra = null)
        {
            left.ForceChirality(Chirality.Con);
            right.ForceChirality(Chirality.Pro);
            Left = left;
            Right = right;
            Chirality = chirality;
            Algebra = algebra ?? ComplexAlgebra;
        }

        /// <summary>
        /// Convenience factory: create a frame with correct chirality encoding.
        /// extent0 = imaginary/Con extent (e.g. height), extent1 = real/Pro extent (e.g. width).
        /// </summary>
        public static Axis Frame(long extent0, long extent1, long resolution, AlgebraEntry[]? algebra = null)
        {
            // Con chirality reads bot as numerator, top as denominator.
            // So to get GetNumerator()=extent0 and GetDenominator()=resolution:
            var left = new Proportion(resolution, extent0, Chirality.Con);
            var right = new Proportion(extent1, resolution, Chirality.Pro);
            return new Axis(left, right, Chirality.Pro, algebra);
        }

        // --- IAlgebraic<Proportion> ---

        /// <summary>
        /// IAlgebraic: get Left (0) or Right (1) as Proportion elements.
        /// </summary>
        public Proportion GetElement(int index) => index switch
        {
            0 => Left,
            1 => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        // --- Algebra operations ---

        /// <summary>
        /// Multiply two Axes at the Proportion level using their algebra.
        /// Returns an Axis with the result packed into Left (imaginary) and Right (real).
        /// Result inherits the left operand's algebra.
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
            return FromProportions(result[0], result[1], a.Chirality, a.Algebra);
        }

        /// <summary>
        /// Construct an Axis from algebra results (Pro chirality Proportions).
        /// Converts the imaginary Proportion into Con perspective for Left.
        /// </summary>
        public static Axis FromProportions(Proportion imaginary, Proportion real, Chirality chirality, AlgebraEntry[]? algebra = null)
        {
            // imaginary is in Pro chirality from algebra. Convert to Con for Left:
            // swap num/den so the value is preserved under the Con perspective.
            var left = new Proportion(imaginary.GetDenominator(), imaginary.GetNumerator(), Chirality.Con);
            return new Axis(left, real, chirality, algebra);
        }

        // --- Fold ---

        /// <summary>
        /// Fold: execute the deferred multiplication. Collapses Grade 2 (Axis) to Grade 1 (Proportion).
        /// Left × Right produces a single Proportion encoding the product of both dimensions.
        /// </summary>
        public Proportion Fold() => Left * Right;

        // --- Child management ---

        /// <summary>
        /// Add a sample as raw ticks. Creates a lightweight child Axis inheriting
        /// this frame's resolution (Unot, Unit) and algebra.
        /// tick0 = imaginary/Con value, tick1 = real/Pro value.
        /// Returns the child so it can be nested into further.
        /// </summary>
        public Axis AddSample(long tick0, long tick1)
        {
            // Build child Proportions with parent's resolution:
            // Con: bot=tick0 → GetNumerator()=tick0, top=Unot → GetDenominator()=Unot
            var left = new Proportion(Unot, tick0, Chirality.Con);
            // Pro: top=tick1 → GetNumerator()=tick1, bot=Unit → GetDenominator()=Unit
            var right = new Proportion(tick1, Unit, Chirality.Pro);
            var child = new Axis(left, right, Chirality, Algebra);
            child.Parent = this;
            _children.Add(child);
            return child;
        }

        /// <summary>
        /// Add a pre-built Axis as a child of this frame.
        /// Sets the parent reference for context inheritance.
        /// Returns the child.
        /// </summary>
        public Axis AddChild(Axis child)
        {
            child.Parent = this;
            _children.Add(child);
            return child;
        }

        /// <summary>
        /// Fold a specific child using its own (inherited) algebra.
        /// Returns the child's Proportion-level fold result.
        /// </summary>
        public Proportion FoldChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _children[index].Fold();
        }

        // --- Long-level access (4 components) for backward compatibility ---
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
