# Core 2 Interpretation Units

This folder contains interpretation-side quantity helpers.

Its role is to bridge native Core 2 quantity structure into more explicit interpreted quantity behavior without abandoning the unit and signature model underneath.

## What Lives Here

This area currently contains quantity-oriented interpretation extensions and related helpers.

## Mental Model

Use this folder when a quantity already exists in Core 2 terms, but now needs interpretation-aware behavior for reading, embedding, or interaction with the rest of the interpretation layer.

This folder should stay close to the native `Core2/Units` model rather than introducing a second unit system.
