using Core3.Binding;
using Core3.Serialization;

namespace Tests.Core3;

public sealed class BindingSerializationTests
{
    [Fact]
    public void Core3JsonSerializer_SerializesBindingAddressKinds_Minimally()
    {
        // Serializes the remaining binding address shapes directly.
        // Position is now the mover-relative numeric parameter, so:
        // 0 means "here", -1 means one step back, and 1/2 means halfway/query midpoint.
        var expectedHereJson = """
{
  "kind": "position",
  "parameter": {
    "kind": "atomic",
    "grade": 0,
    "value": 0,
    "unit": 1
  }
}
""";

        var expectedNameJson = """
{
  "kind": "name",
  "value": "accumulator"
}
""";

        var expectedPriorJson = """
{
  "kind": "position",
  "parameter": {
    "kind": "atomic",
    "grade": 0,
    "value": -1,
    "unit": 1
  }
}
""";

        var expectedMidpointJson = """
{
  "kind": "position",
  "parameter": {
    "kind": "atomic",
    "grade": 0,
    "value": 1,
    "unit": 2
  }
}
""";

        AssertJsonEqual(expectedHereJson, Core3JsonSerializer.Serialize(BindingAddress.At(0)));
        AssertJsonEqual(expectedNameJson, Core3JsonSerializer.Serialize(new BindingAddress.Name("accumulator")));
        AssertJsonEqual(expectedPriorJson, Core3JsonSerializer.Serialize(BindingAddress.At(-1)));
        AssertJsonEqual(expectedMidpointJson, Core3JsonSerializer.Serialize(BindingAddress.At(1, 2)));
    }

    [Fact]
    public void Core3JsonSerializer_SerializesBindingSelectorWithStorage_Minimally()
    {
        // Serializes a selector that reads "where the mover is now" in the family domain
        // and stores that selected value into the local context as "currentItem".
        var expectedJson = """
{
  "domain": "Family",
  "address": {
    "kind": "position",
    "parameter": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 1
    }
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

        var selector = BindingSelector.At(
            BindingDomain.Family,
            0,
            projection: BindingProjection.Whole,
            storeTarget: new BindingStorageTarget(BindingDomain.Context, "currentItem"));

        var json = Core3JsonSerializer.Serialize(selector);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesBoundScalarTemplate_Minimally()
    {
        // Serializes a scalar bound literal with one explicit value, one unit bound
        // from the mover-relative frame position 0, and one named token-unit coupling.
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
        "kind": "position",
        "parameter": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 1
        }
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
                Binding = BindingSelector.At(
                    BindingDomain.Frame,
                    0,
                    projection: BindingProjection.Unit),
                Transform = BindingTransform.OppositeOrientation
            },
            Constraints =
            [
                new BindingConstraint(
                    "unit",
                    BindingSelector.Named(
                        BindingDomain.Token,
                        "accumulator",
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
        // history-bound scalar whose address is one step back and whose unit is orthogonalized.
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
          "kind": "position",
          "parameter": {
            "kind": "atomic",
            "grade": 0,
            "value": -1,
            "unit": 1
          }
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
                    Binding = BindingSelector.At(
                        BindingDomain.History,
                        -1,
                        projection: BindingProjection.Whole)
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
