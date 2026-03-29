using Core3.Binding;
using Core3.Serialization;

namespace Tests.Core3;

public sealed class BindingSerializationTests
{
    [Fact]
    public void Core3JsonSerializer_SerializesBindingAddressKinds_Minimally()
    {
        // Serializes the built-in binding address shapes directly.
        // These are the local "where inside the domain?" descriptors used by binding.
        var expectedCurrentJson = """
{
  "kind": "current"
}
""";

        var expectedNameJson = """
{
  "kind": "name",
  "value": "accumulator"
}
""";

        var expectedSlotJson = """
{
  "kind": "slot",
  "index": 2
}
""";

        var expectedOffsetJson = """
{
  "kind": "offset",
  "value": -1
}
""";

        var expectedNormalizedJson = """
{
  "kind": "normalized",
  "position": 0.5
}
""";

        AssertJsonEqual(expectedCurrentJson, Core3JsonSerializer.Serialize(new BindingAddress.Current()));
        AssertJsonEqual(expectedNameJson, Core3JsonSerializer.Serialize(new BindingAddress.Name("accumulator")));
        AssertJsonEqual(expectedSlotJson, Core3JsonSerializer.Serialize(new BindingAddress.Slot(2)));
        AssertJsonEqual(expectedOffsetJson, Core3JsonSerializer.Serialize(new BindingAddress.Offset(-1)));
        AssertJsonEqual(expectedNormalizedJson, Core3JsonSerializer.Serialize(new BindingAddress.Normalized(0.5m)));
    }

    [Fact]
    public void Core3JsonSerializer_SerializesBindingSelectorWithStorage_Minimally()
    {
        // Serializes a selector that reads the current family member and stores it
        // into the local context as "currentItem" for later steps.
        var expectedJson = """
{
  "domain": "Family",
  "address": {
    "kind": "current"
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
  },
  "storeTarget": {
    "domain": "Context",
    "name": "currentItem"
  }
}
""";

        var selector = new BindingSelector(
            BindingDomain.Family,
            new BindingAddress.Current(),
            BindingProjection.Whole,
            new BindingStorageTarget(BindingDomain.Context, "currentItem"));

        var json = Core3JsonSerializer.Serialize(selector);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesBoundScalarTemplate_Minimally()
    {
        // Serializes a scalar bound literal with one explicit value, one inherited unit,
        // and one coupling constraint from the token accumulator's unit slot.
        var expectedJson = """
{
  "kind": "boundScalarTemplate",
  "materialization": "OnBind",
  "value": {
    "literal": 25,
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
  },
  "unit": {
    "binding": {
      "domain": "Frame",
      "address": {
        "kind": "current"
      },
      "projection": {
        "note": "unit",
        "signal": {
          "note": "negate",
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
                "value": -1,
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
    },
    "transform": {
      "note": "opposite-orientation",
      "signal": {
        "note": "negate",
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
              "value": -1,
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
  },
  "constraints": [
    {
      "targetPath": "unit",
      "materialization": "OnBind",
      "source": {
        "domain": "Token",
        "address": {
          "kind": "name",
          "value": "accumulator"
        },
        "projection": {
          "note": "unit",
          "signal": {
            "note": "negate",
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
                  "value": -1,
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

        var template = new BoundScalarTemplate
        {
            Value = new BoundSlot<long>
            {
                Literal = 25
            },
            Unit = new BoundSlot<long>
            {
                Binding = BindingSelector.Current(
                    BindingDomain.Frame,
                    BindingProjection.Unit),
                Transform = BindingTransform.OppositeOrientation
            },
            Constraints =
            [
                new BindingConstraint(
                    "unit",
                    new BindingSelector(
                        BindingDomain.Token,
                        new BindingAddress.Name("accumulator"),
                        BindingProjection.Unit),
                    BindingTransform.Identity)
            ]
        };

        var json = Core3JsonSerializer.Serialize(template);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesBoundCompositeTemplate_Minimally()
    {
        // Serializes a recursive bound template: one literal scalar plus one
        // history-bound scalar whose unit is orthogonalized at bind time.
        var expectedJson = """
{
  "kind": "boundCompositeTemplate",
  "materialization": "OnStep",
  "recessive": {
    "kind": "boundScalarTemplate",
    "materialization": "OnBind",
    "value": {
      "literal": 10,
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
    },
    "unit": {
      "literal": 1,
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
    },
    "constraints": []
  },
  "dominant": {
    "kind": "boundScalarTemplate",
    "materialization": "OnBind",
    "value": {
      "binding": {
        "domain": "History",
        "address": {
          "kind": "offset",
          "value": -1
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
    },
    "unit": {
      "literal": 1,
      "transform": {
        "note": "orthogonalize",
        "signal": {
          "note": "orthogonal",
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
                "value": 0,
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
                "value": 1,
                "unit": 1
              }
            }
          }
        }
      }
    },
    "constraints": []
  },
  "constraints": []
}
""";

        var template = new BoundCompositeTemplate(
            new BoundScalarTemplate
            {
                Value = new BoundSlot<long>
                {
                    Literal = 10
                },
                Unit = new BoundSlot<long>
                {
                    Literal = 1
                }
            },
            new BoundScalarTemplate
            {
                Value = new BoundSlot<long>
                {
                    Binding = new BindingSelector(
                        BindingDomain.History,
                        new BindingAddress.Offset(-1),
                        BindingProjection.Whole)
                },
                Unit = new BoundSlot<long>
                {
                    Literal = 1,
                    Transform = BindingTransform.Orthogonalize
                }
            })
        {
            Materialization = BindingMaterialization.OnStep
        };

        var json = Core3JsonSerializer.Serialize(template);

        AssertJsonEqual(expectedJson, json);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson) =>
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));

    private static string Normalize(string json) =>
        json.Trim().ReplaceLineEndings("\n");
}
