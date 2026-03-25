# Elements

This folder will hold the core primitives of `Core3`.

For now it is intentionally sparse. The aim is to keep only the most single-minded foundational elements here, and push richer interpretation and higher-level structure outward until the primitive layer is settled.

The current shape is:

- an `IElement` always has a `Start` and an `End`
- those are both `ICarrier`s
- every carrier also has a side: `Inbound` or `Outbound`

That means the dual perspective is not something added later for display. It is part of the primitive readout of every element.

At the moment:

- `RawExtent` is the most basic ordered support
- `Proportion` is a `RawExtent` read through a pin position
- `Pin` joins two carriers at one located site and then reads:
  - one inbound carrier
  - one outbound carrier

The important point is that a pin does not create meaning by itself. It places two already-ordered sides into one relation. Inbound and outbound then read the same raw support from opposite roles.

The current bootstrap implementation is long-backed:

- `LongCarrier` stores one raw value
- `CarrierSide` decides whether that raw value is read directly or mirrored

This is meant to scale later to higher grades without changing the basic picture: all elements remain dual-perspective, and a pin remains a relation between one inbound side and one outbound side.
