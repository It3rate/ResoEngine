# Core 2 Interpretation Analysis

This folder contains structural analysis over interpreted Core 2 objects.

The emphasis here is not generic graph analysis detached from the rest of the system. The analyses are meant to stay expressed in Core 2 terms: carriers, sites, routes, junctions, attachments, and other native structural readings.

## What Lives Here

This area handles things like:

- carrier-pin graph analysis
- structural route discovery
- junction summaries
- attachment and incident summaries
- structural context adapters used by symbolic evaluation

## Mental Model

Use this folder when you want to ask:

- what carrier does this anchor actually resolve onto
- what kind of junction is this site
- what routes exist through this structure
- what structural facts can be fed back into symbolic evaluation

This is one of the main places where interpretation and symbolic evaluation connect.
