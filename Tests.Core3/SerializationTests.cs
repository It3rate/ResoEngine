using Core3.Binding;
using Core3.Engine;
using Core3.Runtime;
using Core3.Serialization;

namespace Tests.Core3;

public sealed class SerializationTests
{
    [Fact]
    public void Core3JsonSerializer_SerializesGradedElementsAndContexts()
    {
        // Serializes one ordered runtime context: frame 4/4 with two members 1/2 and 3/4.
        // Approximate math: an ordered family read in quarter-calibration with members
        // equivalent to 2/4 and 3/4.
        var expectedJson = """
{
  "kind": "operationContext",
  "isOrdered": true,
  "frame": {
    "kind": "atomic",
    "grade": 0,
    "value": 4,
    "unit": 4
  },
  "members": [
    {
      "kind": "atomic",
      "grade": 0,
      "value": 1,
      "unit": 2
    },
    {
      "kind": "atomic",
      "grade": 0,
      "value": 3,
      "unit": 4
    }
  ]
}
""";

        var context = EngineOperationContext.Create(
            new AtomicElement(4, 4),
            [
                new AtomicElement(1, 2),
                new AtomicElement(3, 4)
            ],
            isOrdered: true);

        var json = Core3JsonSerializer.Serialize(context);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_CanIncludeDerivedReferenceReads()
    {
        // Serializes a frame-borrowing view with derived reads enabled.
        // Approximate math: read 7/1 through calibration 10/10, preserving the host
        // frame plus the borrowed read 70/10.
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
  "read": {
    "kind": "atomic",
    "grade": 0,
    "value": 70,
    "unit": 10
  }
}
""";

        var frame = new CompositeElement(
            new AtomicElement(10, 10),
            new AtomicElement(3, 10));
        var view = new EngineView(frame, new AtomicElement(7, 1));

        var json = Core3JsonSerializer.Serialize(
            view,
            new Core3JsonSerializerOptions
            {
                IncludeDerived = true
            });

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesOperationAttachmentsWithSignals()
    {
        // Serializes one attached Add law at a carrier site.
        // Approximate math: a one-input accumulator step whose selector and output
        // transform are both expressed as Core3-valued binding signals instead of enums.
        var expectedJson = """
{
  "kind": "operationAttachment",
  "site": {
    "kind": "Carrier",
    "name": "accumulate"
  },
  "law": {
    "name": "Add"
  },
  "inputs": [
    {
      "name": "left",
      "materialization": "OnRead",
      "selector": {
        "domain": "Token",
        "address": {
          "kind": "name",
          "value": "accumulator"
        },
        "projection": {
          "note": "whole",
          "signal": {
            "note": "identity",
            "value": {
              "kind": "composite",
              "grade": 2,
              "recessive": {
                "kind": "composite",
                "grade": 1,
                "recessive": {
                  "kind": "atomic",
                  "grade": 0,
                  "value": 1,
                  "unit": 1
                },
                "dominant": {
                  "kind": "atomic",
                  "grade": 0,
                  "value": 1,
                  "unit": 1
                }
              },
              "dominant": {
                "kind": "composite",
                "grade": 1,
                "recessive": {
                  "kind": "atomic",
                  "grade": 0,
                  "value": 1,
                  "unit": 1
                },
                "dominant": {
                  "kind": "atomic",
                  "grade": 0,
                  "value": 0,
                  "unit": 1
                }
              }
            }
          }
        }
      }
    }
  ],
  "outputs": [
    {
      "name": "sum",
      "target": {
        "domain": "Token",
        "name": "accumulator"
      },
      "transform": {
        "note": "identity",
        "signal": {
          "note": "identity",
          "value": {
            "kind": "composite",
            "grade": 2,
            "recessive": {
              "kind": "composite",
              "grade": 1,
              "recessive": {
                "kind": "atomic",
                "grade": 0,
                "value": 1,
                "unit": 1
              },
              "dominant": {
                "kind": "atomic",
                "grade": 0,
                "value": 1,
                "unit": 1
              }
            },
            "dominant": {
              "kind": "composite",
              "grade": 1,
              "recessive": {
                "kind": "atomic",
                "grade": 0,
                "value": 1,
                "unit": 1
              },
              "dominant": {
                "kind": "atomic",
                "grade": 0,
                "value": 0,
                "unit": 1
              }
            }
          }
        }
      }
    }
  ]
}
""";

        var attachment = new OperationAttachment(
            new OperationSite(OperationSiteKind.Carrier, "accumulate"),
            new OperationLawReference("Add"),
            [
                new OperationInputBinding(
                    "left",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "accumulator",
                        BindingProjection.Whole))
            ],
            [
                new OperationOutputBinding(
                    "sum",
                    new BindingStorageTarget(BindingDomain.Token, "accumulator"),
                    BindingTransform.Identity)
            ]);

        var json = Core3JsonSerializer.Serialize(attachment);

        AssertJsonEqual(expectedJson, json);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson) =>
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));

    private static string Normalize(string json) =>
        json.Trim().ReplaceLineEndings("\n");
}
