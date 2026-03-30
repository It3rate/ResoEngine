using Core3.Binding;
using Core3.Serialization;

namespace Tests.Core3;

public sealed class TraversalMachineSerializationTests
{
    [Fact]
    public void Core3JsonSerializer_SerializesAccumulatorLoopMachine_Minimally()
    {
        // Serializes a small loop machine:
        // - one accumulator register carried by the moving token/trolley
        // - one accumulate site that reads the current family item at position 0
        // - one continue site that looks one step ahead at position 1
        //
        // Approximate math:
        //   accumulator := accumulator + currentItem
        //   continue while nextItem exists
        var expectedJson = """
{
  "kind": "traversalMachine",
  "name": "sum-loop",
  "entrySiteName": "accumulate",
  "registers": [
    {
      "name": "accumulator",
      "template": {
        "kind": "boundScalarTemplate",
        "materialization": "OnBind",
        "value": {
          "literal": 0,
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
      }
    }
  ],
  "attachments": [
    {
      "kind": "operationAttachment",
      "site": {
        "kind": "Carrier",
        "name": "accumulate",
        "address": {
          "kind": "position",
          "parameter": {
            "kind": "atomic",
            "grade": 0,
            "value": 1,
            "unit": 2
          }
        }
      },
      "law": {
        "name": "Add"
      },
      "inputs": [
        {
          "name": "accumulator",
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
        },
        {
          "name": "currentItem",
          "materialization": "OnRead",
          "selector": {
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
    },
    {
      "kind": "operationAttachment",
      "site": {
        "kind": "Boundary",
        "name": "continue"
      },
      "law": {
        "name": "ContinueWhileNextMemberExists"
      },
      "inputs": [
        {
          "name": "nextItem",
          "materialization": "OnRead",
          "selector": {
            "domain": "Family",
            "address": {
              "kind": "position",
              "parameter": {
                "kind": "atomic",
                "grade": 0,
                "value": 1,
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
              "name": "nextItem"
            }
          }
        }
      ],
      "outputs": [
        {
          "name": "route",
          "target": {
            "domain": "Result",
            "name": "route"
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
  ]
}
""";

        var machine = new TraversalMachineDefinition(
            "sum-loop",
            "accumulate",
            [
                new TraversalRegister(
                    "accumulator",
                    new BoundScalarTemplate
                    {
                        Value = new BoundSlot<long> { Literal = 0 },
                        Unit = new BoundSlot<long> { Literal = 1 }
                    })
            ],
            [
                new OperationAttachment(
                    new OperationSite(
                        OperationSiteKind.Carrier,
                        "accumulate",
                        BindingAddress.At(1, 2)),
                    new OperationLawReference("Add"),
                    [
                        new OperationInputBinding(
                            "accumulator",
                            BindingSelector.Named(
                                BindingDomain.Token,
                                "accumulator",
                                BindingProjection.Whole)),
                        new OperationInputBinding(
                            "currentItem",
                            BindingSelector.At(
                                BindingDomain.Family,
                                0,
                                projection: BindingProjection.Whole,
                                storeTarget: new BindingStorageTarget(BindingDomain.Context, "currentItem")))
                    ],
                    [
                        new OperationOutputBinding(
                            "sum",
                            new BindingStorageTarget(BindingDomain.Token, "accumulator"),
                            BindingTransform.Identity)
                    ]),
                new OperationAttachment(
                    new OperationSite(OperationSiteKind.Boundary, "continue"),
                    new OperationLawReference("ContinueWhileNextMemberExists"),
                    [
                        new OperationInputBinding(
                            "nextItem",
                            BindingSelector.At(
                                BindingDomain.Family,
                                1,
                                projection: BindingProjection.Whole,
                                storeTarget: new BindingStorageTarget(BindingDomain.Context, "nextItem")))
                    ],
                    [
                        new OperationOutputBinding(
                            "route",
                            new BindingStorageTarget(BindingDomain.Result, "route"),
                            BindingTransform.Identity)
                    ])
            ]);

        var json = Core3JsonSerializer.Serialize(machine);

        AssertJsonEqual(expectedJson, json);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson) =>
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));

    private static string Normalize(string json) =>
        json.Trim().ReplaceLineEndings("\n");
}
