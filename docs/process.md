# Delivery app business processes

DDDDelivery supports shops and any businesses that need to manage distribution of goods they sell.
Its intended users are owners or managers of such shops, not the customers.

## Process Overview

DDDDelivery tracks orders as central entities.
Orders are descriptions of requests made by a **customer** to provide him with a specified **products** for some price.
Orders cannot be divided to be shipped separately.
Orders are tracked through their whole lifecycle, from being requested to shipped or cancelled.

DDDDelivery provides its users with methods to manage that lifecycle.
The system is designed in a way enabling easy automation of that management and integration with external systems, i.e. e-shops and payment providers.

DDDDelivery in MVP version does not distinguish between its users, so every one is "trusted", but not all-allowed.

### Orders lifecycle

Each order, as tracked by DDDDelivery, can be in exactly one of the following states (with their codenames marked):

- _pending_,
- actively being _processed_,
- _awaiting_ shipment,
- _shipped_,
- _delivered_,
- _cancelled_.

Orders in the first three of those states are also referred to as _active_.

New orders always appear as _pending_ (although it is possible to extend DDDDelivery to import externally saved deliveries with other states).

From _pending_ state delivery can be moved either to _processed_ or _cancelled_.

From _processed_ state delivery can be moved either to _awaiting_ or _cancelled_.

From _awaiting_ state delivery can be moved either to _shipped_ or _cancelled_.

From _shipped_ state delivery can only be moved to _delivered_.

Any other transitions are impossible.
Noteworthily, _cancelled_ an _delivered_ states are final and are considered archived and immutable.
In other words, any _active_ order can be cancelled or uplifted to the next state.

## Providing statistics, analyses and reports

An additional, yet first-class area that DDDDelivery handles is provid in-depth summaries and statistics.
Some examples of those are:

- all orders by state
- prolonging shipments
- prolonging processing
- orders by customer

## Customers and products

To enrich orders management, DDDDelivery also manages lists of customers and products.
However, those are treated as second-class entites: only minimal needed information are stored and management options are limited.
