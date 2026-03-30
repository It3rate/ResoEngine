using Core3.Binding;
using Core3.Engine;
using Core3.Operations;
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
        // Route iterator:
        //   0/4 means the mover is at the start of a four-step route sample.
        var expectedJson = """
{
  "kind": "traversalMachine",
  "name": "sum-loop",
  "entrySiteName": "accumulate",
  "mover": {
    "kind": "traversalMover",
    "name": "family-cursor",
    "position": {
      "kind": "atomic",
      "grade": 0,
      "value": 0,
      "unit": 4
    }
  },
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

        var mover = new TraversalMover(
            "family-cursor",
            new AtomicElement(0, 4));

        var machine = CreateAccumulatorLoopMachine(mover);

        var json = Core3JsonSerializer.Serialize(machine);

        AssertJsonEqual(expectedJson, json);
    }

    [Fact]
    public void Core3JsonSerializer_SerializesTraversalStepResult_Minimally()
    {
        // Serializes one live traversal step of the accumulator loop.
        // Approximate math:
        //   accumulator := 0 + 1
        //   next item exists, so route := continue and mover advances from 0/3 to 1/3
        var expectedJson = """
{
  "kind": "traversalStepResult",
  "state": {
    "kind": "traversalRuntimeState",
    "machineName": "sum-loop",
    "mover": {
      "kind": "traversalMover",
      "name": "family-cursor",
      "position": {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 3
      }
    },
    "token": {
      "accumulator": {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      }
    },
    "context": {
      "currentItem": {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      },
      "nextItem": {
        "kind": "atomic",
        "grade": 0,
        "value": 2,
        "unit": 1
      }
    },
    "result": {
      "route": {
        "kind": "atomic",
        "grade": 0,
        "value": 1,
        "unit": 1
      }
    }
  },
  "encounters": [
    {
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
      "inputs": {
        "accumulator": {
          "kind": "atomic",
          "grade": 0,
          "value": 0,
          "unit": 1
        },
        "currentItem": {
          "kind": "atomic",
          "grade": 0,
          "value": 1,
          "unit": 1
        }
      }
    },
    {
      "site": {
        "kind": "Boundary",
        "name": "continue"
      },
      "law": {
        "name": "ContinueWhileNextMemberExists"
      },
      "inputs": {
        "nextItem": {
          "kind": "atomic",
          "grade": 0,
          "value": 2,
          "unit": 1
        }
      }
    }
  ]
}
""";

        var machine = CreateAccumulatorLoopMachine(
            new TraversalMover("family-cursor", new AtomicElement(0, 3)));
        var family = new EngineFamily(new AtomicElement(0, 1));
        family.AddMember(new AtomicElement(1, 1));
        family.AddMember(new AtomicElement(2, 1));
        family.AddMember(new AtomicElement(3, 1));

        var state = TraversalRuntime.CreateInitial(machine);
        Assert.True(TraversalRuntime.TryStep(state, family, out var step));

        var json = Core3JsonSerializer.Serialize(Assert.IsType<TraversalStepResult>(step));

        AssertJsonEqual(expectedJson, json);
    }

    private static void AssertJsonEqual(string expectedJson, string actualJson) =>
        Assert.Equal(Normalize(expectedJson), Normalize(actualJson));

    private static string Normalize(string json) =>
        json.Trim().ReplaceLineEndings("\n");

    private static TraversalMachineDefinition CreateAccumulatorLoopMachine(TraversalMover mover) =>
        new(
            "sum-loop",
            "accumulate",
            mover,
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
}
