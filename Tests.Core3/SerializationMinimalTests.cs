using Core3.Engine;
using Core3.Operations;
using Core3.Runtime;
using Core3.Serialization;

namespace Tests.Core3;

public sealed class SerializationMinimalTests
{
    [Fact]
    public void Core3JsonSerializer_SerializesReference_MinimallyByDefault()
    {
        // Serializes a reference relation without any derived readout fields.
        // Approximate math: keep the instruction "read 7 through the frame (10, 3)"
        // without serializing the computed 70/10 read yet.
        var expectedJson = """
{
  "kind": "view",
  "frame": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 10,
      "unit": 10
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": 10
    }
  },
  "subject": {
    "kind": "atomic",
    "grade": 0,
    "value": 7,
    "unit": 1
  }
}
""";

        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var view = new EngineView(frame, new AtomicElement(7, 1));

        var json = Core3JsonSerializer.Serialize(view);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesHostedPinResultWithTension_Minimally()
    {
        // Serializes a hosted pin request that cannot settle onto one exact route
        // position, preserving the unresolved placement and local sides.
        var expectedJson = """
{
  "kind": "hostedPinResult",
  "isExact": false,
  "host": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 10,
      "unit": 1
    }
  },
  "requestedPosition": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 2,
      "unit": 1
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": -1
    }
  },
  "resolvedPosition": {
    "kind": "atomic",
    "grade": 0,
    "value": 3,
    "unit": 0
  },
  "inbound": {
    "kind": "atomic",
    "grade": 0,
    "value": 3,
    "unit": 0
  },
  "outbound": {
    "kind": "atomic",
    "grade": 0,
    "value": 7,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 2,
      "unit": 1
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": -1
    }
  },
  "note": "Hosted pin preserved a contrastive or unresolved ratio position. | Subtraction preserved unresolved support from the aligned pair."
}
""";

        var host = new CompositeElement(
            new AtomicElement(0, 1),
            new AtomicElement(10, 1));
        var contrastiveRatio = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));

        var json = Core3JsonSerializer.Serialize(
            EnginePin.ResolveHostedWithTension(host, contrastiveRatio));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesMultiplyOperationAndFoldedResult_Minimally()
    {
        // First serialization: the operation result for 3 * 4 in a unit frame.
        // Second serialization: the folded scalar result 12.
        var expectedOperationJson = """
{
  "kind": "operationResult",
  "operationName": "Multiply",
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 1
    },
    "members": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 3,
        "unit": 1
      },
      {
        "kind": "atomic",
        "grade": 0,
        "value": 4,
        "unit": 1
      }
    ]
  },
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 12,
    "unit": 1
  },
  "resultFrame": {
    "kind": "atomic",
    "grade": 0,
    "value": 1,
    "unit": 1
  }
}
""";

        var expectedFoldedResultJson = """
{
  "kind": "atomic",
  "grade": 0,
  "value": 12,
  "unit": 1
}
""";

        var frame = new AtomicElement(1, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(3, 1),
            new AtomicElement(4, 1)
        };

        Assert.True(EngineOperations.TryMultiplyWithProvenance(frame, members, out var operationResult));

        var finalizedResult = Assert.IsType<EngineOperationResult>(operationResult);
        var operationJson = Core3JsonSerializer.Serialize(finalizedResult);
        var foldedResultJson = Core3JsonSerializer.Serialize(finalizedResult.Result);

        AssertJsonEqual(expectedOperationJson, operationJson);
        AssertJsonEqual(expectedFoldedResultJson, foldedResultJson);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingReadResult_Minimally()
    {
        // Serializes a family read where one member cannot be calibrated exactly
        // into the active frame.
        var expectedJson = """
{
  "kind": "readResult",
  "isExact": false,
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    },
    "members": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      },
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 0
      }
    ]
  },
  "reads": [
    {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 1
    },
    {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 0
    }
  ],
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 0
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    }
  },
  "note": "Calibration preserved unresolved support because one or both unit slots were unresolved."
}
""";

        var family = new EngineFamily(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        Assert.True(family.TryReadAllWithTension(out var readResult));

        var json = Core3JsonSerializer.Serialize(Assert.IsType<EngineReadResult>(readResult));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedReferenceOutcome_WhenReadIsNotExact()
    {
        // Serializes a reference with derived view enabled when the borrowed read
        // remains unresolved under the frame calibration.
        var expectedJson = """
{
  "kind": "view",
  "frame": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 10,
      "unit": 10
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": 10
    }
  },
  "subject": {
    "kind": "atomic",
    "grade": 0,
    "value": 7,
    "unit": 0
  },
  "calibration": {
    "kind": "atomic",
    "grade": 0,
    "value": 10,
    "unit": 10
  },
  "existingReadout": {
    "kind": "atomic",
    "grade": 0,
    "value": 3,
    "unit": 10
  },
  "readOutcome": {
    "kind": "elementOutcome",
    "isExact": false,
    "result": {
      "kind": "atomic",
      "grade": 0,
      "value": 70,
      "unit": 0
    },
    "tension": {
      "kind": "composite",
      "grade": 1,
      "recessive": {
        "kind": "atomic",
        "grade": 0,
        "value": 7,
        "unit": 0
      },
      "dominant": {
        "kind": "atomic",
        "grade": 0,
        "value": 10,
        "unit": 10
      }
    },
    "note": "Calibration preserved unresolved support because one or both unit slots were unresolved.",
    "survivorCount": 1,
    "outboundResults": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 70,
        "unit": 0
      }
    ],
    "rawPair": {
      "kind": "elementPairOutcome",
      "isExact": true,
      "left": {
        "kind": "atomic",
        "grade": 0,
        "value": 7,
        "unit": 0
      },
      "right": {
        "kind": "atomic",
        "grade": 0,
        "value": 10,
        "unit": 10
      },
      "survivorCount": 2,
      "outboundResults": [
        {
          "kind": "atomic",
          "grade": 0,
          "value": 7,
          "unit": 0
        },
        {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      ]
    }
  }
}
""";

        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var view = new EngineView(frame, new AtomicElement(7, 0));

        var json = Core3JsonSerializer.Serialize(
            view,
            new Core3JsonSerializerOptions { IncludeDerived = true });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedOperationResultOutboundPieces()
    {
        var expectedJson = """
{
  "kind": "operationResult",
  "operationName": "Multiply",
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 1
    },
    "members": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 3,
        "unit": 1
      },
      {
        "kind": "atomic",
        "grade": 0,
        "value": 4,
        "unit": 1
      }
    ]
  },
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 12,
    "unit": 1
  },
  "resultFrame": {
    "kind": "atomic",
    "grade": 0,
    "value": 1,
    "unit": 1
  },
  "originLawName": "Multiply",
  "outboundPieces": [
    {
      "result": {
        "kind": "atomic",
        "grade": 0,
        "value": 12,
        "unit": 1
      },
      "carrier": {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      },
      "sourceMemberIndices": [
        0,
        1
      ]
    }
  ],
  "readResult": {
    "kind": "atomic",
    "grade": 0,
    "value": 12,
    "unit": 1
  },
  "resultBoundaryAxis": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 11,
      "unit": 1
    }
  }
}
""";

        var frame = new AtomicElement(1, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(3, 1),
            new AtomicElement(4, 1)
        };

        Assert.True(EngineOperations.TryMultiplyWithProvenance(frame, members, out var operationResult));

        var json = Core3JsonSerializer.Serialize(
            Assert.IsType<EngineOperationResult>(operationResult),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedOperationResultPreservedStructure()
    {
        var left = Core3TestHelpers.CreateAxisLikeNumber(1, 1, 2, 1);
        var right = Core3TestHelpers.CreateAxisLikeNumber(1, 1, 4, 1);
        Assert.True(EngineOperations.TryMultiplyWithProvenance(left, [left, right], out var operationResult));

        var json = Core3JsonSerializer.Serialize(
            Assert.IsType<EngineOperationResult>(operationResult),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        Assert.Contains("\"preservedStructure\"", json);
        Assert.Contains("\"value\": 2", json);
        Assert.Contains("\"value\": 8", json);
        Assert.Contains("\"value\": 4", json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedAddOperationResultPreservedStructure()
    {
        var frame = new AtomicElement(0, 1);
        var members = new GradedElement[]
        {
            new AtomicElement(1, 1),
            new AtomicElement(1, 0)
        };

        Assert.True(EngineOperations.TryAddWithTension(frame, members, out var operationResult));

        var json = Core3JsonSerializer.Serialize(
            Assert.IsType<EngineOperationResult>(operationResult),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        Assert.Contains("\"preservedStructure\"", json);
        Assert.Contains("\"value\": 1", json);
        Assert.Contains("\"unit\": 0", json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedBooleanResultOutboundPieces()
    {
        var expectedJson = """
{
  "kind": "booleanResult",
  "operation": "Xor",
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "composite",
      "grade": 1,
      "recessive": {
        "kind": "atomic",
        "grade": 0,
        "value": 0,
        "unit": 10
      },
      "dominant": {
        "kind": "atomic",
        "grade": 0,
        "value": 10,
        "unit": 10
      }
    },
    "members": [
      {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 3,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 5,
          "unit": 10
        }
      }
    ]
  },
  "pieces": [
    {
      "result": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 3,
          "unit": 10
        }
      },
      "carrier": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "sourceMemberIndices": [
        0
      ]
    },
    {
      "result": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 5,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "carrier": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "sourceMemberIndices": [
        0
      ]
    }
  ],
  "originLawName": "Xor",
  "outboundPieces": [
    {
      "result": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 3,
          "unit": 10
        }
      },
      "carrier": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "sourceMemberIndices": [
        0
      ]
    },
    {
      "result": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 5,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "carrier": {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      "sourceMemberIndices": [
        0
      ]
    }
  ]
}
""";

        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = Core3TestHelpers.CreateSegmentFrame(3, 5, 10);

        Assert.True(EngineOperations.TryBoolean(
            frame,
            [primary, secondary],
            EngineBooleanOperation.Xor,
            out var result));

        var json = Core3JsonSerializer.Serialize(
            Assert.IsType<EngineBooleanResult>(result),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingFoldOutcome_Minimally()
    {
        // Serializes a grade-1 ratio fold that cannot stay on one resolved carrier.
        // Approximate math: fold (3 / -1) over (2 / 1), preserving the unresolved
        // result 3/0 together with the original ratio as the held tension source.
        var expectedJson = """
{
  "kind": "elementOutcome",
  "isExact": false,
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 3,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 2,
      "unit": 1
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": -1
    }
  },
  "note": "Ratio fold preserved carrier contrast as unresolved support."
}
""";

        var contrastive = new CompositeElement(
            new AtomicElement(2, 1),
            new AtomicElement(3, -1));

        var json = Core3JsonSerializer.Serialize(contrastive.Fold());

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingAlignmentOutcome_Minimally()
    {
        // Serializes an alignment request that cannot stay on one resolved carrier.
        // Approximate math: align 1/2 with 1/-4, preserving both projected reads at
        // quarter-like support while keeping the original pair as held tension.
        var expectedJson = """
{
  "kind": "elementPairOutcome",
  "isExact": false,
  "left": {
    "kind": "atomic",
    "grade": 0,
    "value": 4,
    "unit": 0
  },
  "right": {
    "kind": "atomic",
    "grade": 0,
    "value": 4,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": -4
    }
  },
  "note": "Alignment preserved carrier contrast as unresolved support."
}
""";

        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var json = Core3JsonSerializer.Serialize(left.Align(right));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedElementPairOutcomeOutboundResults()
    {
        var expectedJson = """
{
  "kind": "elementPairOutcome",
  "isExact": false,
  "left": {
    "kind": "atomic",
    "grade": 0,
    "value": 4,
    "unit": 0
  },
  "right": {
    "kind": "atomic",
    "grade": 0,
    "value": 4,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": -4
    }
  },
  "note": "Alignment preserved carrier contrast as unresolved support.",
  "survivorCount": 2,
  "outboundResults": [
    {
      "kind": "atomic",
      "grade": 0,
      "value": 4,
      "unit": 0
    },
    {
      "kind": "atomic",
      "grade": 0,
      "value": 4,
      "unit": 0
    }
  ]
}
""";

        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var json = Core3JsonSerializer.Serialize(
            left.Align(right),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingAdditionOutcome_Minimally()
    {
        // Serializes an addition that cannot settle onto one resolved carrier.
        // Approximate math: add 1/2 and 1/-4, preserving the unresolved sum 8/0
        // and the original pair as held tension.
        var expectedJson = """
{
  "kind": "elementOutcome",
  "isExact": false,
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 8,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": -4
    }
  },
  "note": "Addition preserved unresolved support from the aligned pair."
}
""";

        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var json = Core3JsonSerializer.Serialize(left.Add(right));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesDerivedElementOutcomeOutboundResults()
    {
        var expectedJson = """
{
  "kind": "elementOutcome",
  "isExact": false,
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 8,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": -4
    }
  },
  "note": "Addition preserved unresolved support from the aligned pair.",
  "survivorCount": 1,
  "outboundResults": [
    {
      "kind": "atomic",
      "grade": 0,
      "value": 8,
      "unit": 0
    }
  ],
  "rawPair": {
    "kind": "elementPairOutcome",
    "isExact": true,
    "left": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    "right": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": -4
    },
    "survivorCount": 2,
    "outboundResults": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 2
      },
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": -4
      }
    ]
  }
}
""";

        var left = new AtomicElement(1, 2);
        var right = new AtomicElement(1, -4);

        var json = Core3JsonSerializer.Serialize(
            left.Add(right),
            new Core3JsonSerializerOptions { IncludeDerived = true });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingMultiplyOutcome_Minimally()
    {
        // Serializes a multiply that cannot settle because one factor has
        // unresolved support.
        // Approximate math: multiply 2/3 by 4/0, preserving the unresolved
        // product 8/0 and the original pair as held tension.
        var expectedJson = """
{
  "kind": "elementOutcome",
  "isExact": false,
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 8,
    "unit": 0
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 2,
      "unit": 3
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 4,
      "unit": 0
    }
  },
  "note": "Multiplication preserved unresolved support because one or both unit slots were unresolved."
}
""";

        var left = new AtomicElement(2, 3);
        var right = new AtomicElement(4, 0);

        var json = Core3JsonSerializer.Serialize(left.Multiply(right));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingOperationResult_Minimally()
    {
        // Serializes a family add that stays lawful but unresolved.
        // Approximate math: read 1/1 and 1/0 in frame 0/1, then preserve the
        // unresolved sum 2/0 with the earlier calibration tension still attached
        // and the combined note preserved.
        var expectedJson = """
{
  "kind": "operationResult",
  "isExact": false,
  "operationName": "Add",
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    },
    "members": [
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      },
      {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 0
      }
    ]
  },
  "result": {
    "kind": "atomic",
    "grade": 0,
    "value": 2,
    "unit": 0
  },
  "resultFrame": {
    "kind": "atomic",
    "grade": 0,
    "value": 0,
    "unit": 1
  },
  "tension": {
    "kind": "composite",
    "grade": 1,
    "recessive": {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 0
    },
    "dominant": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    }
  },
  "note": "Calibration preserved unresolved support because one or both unit slots were unresolved. | Addition preserved unresolved support from the aligned pair."
}
""";

        var family = new EngineFamily(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(1, 0));

        Assert.True(family.TryAddAllWithTension(out var result));

        var json = Core3JsonSerializer.Serialize(Assert.IsType<EngineOperationResult>(result));

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTensionBearingBooleanResult_Minimally()
    {
        // Serializes a binary boolean read when one compared segment endpoint
        // cannot be placed exactly on the current carrier.
        // Approximate math: compare [0,10] AND [3,5?] in frame [0,10], where the
        // second segment's dominant endpoint is unresolved, so the result stays
        // lawful but produces no settled pieces yet.
        var expectedJson = """
{
  "kind": "booleanResult",
  "isExact": false,
  "operation": "And",
  "context": {
    "kind": "operationContext",
    "isOrdered": true,
    "frame": {
      "kind": "composite",
      "grade": 1,
      "recessive": {
        "kind": "atomic",
        "grade": 0,
        "value": 0,
        "unit": 10
      },
      "dominant": {
        "kind": "atomic",
        "grade": 0,
        "value": 10,
        "unit": 10
      }
    },
    "members": [
      {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 10,
          "unit": 10
        }
      },
      {
        "kind": "composite",
        "grade": 1,
        "recessive": {
          "kind": "atomic",
          "grade": 0,
          "value": 3,
          "unit": 10
        },
        "dominant": {
          "kind": "atomic",
          "grade": 0,
          "value": 50,
          "unit": 0
        }
      }
    ]
  },
  "pieces": [],
  "tension": {
    "kind": "composite",
    "grade": 2,
    "recessive": {
      "kind": "composite",
      "grade": 1,
      "recessive": {
        "kind": "atomic",
        "grade": 0,
        "value": 3,
        "unit": 10
      },
      "dominant": {
        "kind": "atomic",
        "grade": 0,
        "value": 5,
        "unit": 0
      }
    },
    "dominant": {
      "kind": "composite",
      "grade": 1,
      "recessive": {
        "kind": "atomic",
        "grade": 0,
        "value": 0,
        "unit": 10
      },
      "dominant": {
        "kind": "atomic",
        "grade": 0,
        "value": 10,
        "unit": 10
      }
    }
  },
  "note": "Composite calibration preserved child tension. | Boolean projection preserved unresolved support because one or more segment endpoints could not be placed exactly on the current carrier."
}
""";

        var frame = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var primary = Core3TestHelpers.CreateSegmentFrame(0, 10, 10);
        var secondary = new CompositeElement(
            new AtomicElement(3, 10),
            new AtomicElement(5, 0));

        Assert.True(EngineOperations.TryBooleanWithTension(
            frame,
            [primary, secondary],
            EngineBooleanOperation.And,
            out var result));

        var json = Core3JsonSerializer.Serialize(Assert.IsType<EngineBooleanResult>(result));

        AssertJsonEqual(expectedJson, json);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson) =>
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));

    private static string Normalize(string json) =>
        json.Trim().ReplaceLineEndings("\n");
}
