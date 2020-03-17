# EventDrivenSample

Tiny event driven sample application

**NOTE** It should go without saying that this **is not production ready**.

## About the solution

The solution is comprised of 4 projects

- StoreFront - web application that allows for the creation, delivery or cancellation of orders. Performing these actions will result in events being published.
- Operations - Subscribes to order events, applying kind of streaming logic, to act upon the way the store is working.
- Rewards - Subscribes to order events in order to keep the information about customers purchases updated (eventual consistency), using that information to attribute rewards to loyal customers.
- Events - Contains the events that are used in the solution. Also contains interfaces (and implementations) for publishing and subscribing to events.

Again, don't take this structure as something particularly thought out, it's just a relatively simple way to see things in action.

In the root of the solution, there's a Docker Compose file to spin up the necessary dependencies, which are PostgreSQL and Kafka.

Using JSON serialization for the events, good enough for demo purposes. For production scenarios, something like ProtoBuf or Avro are probably better options.

## Main topics

The main topics to be observed in this solution are:

- Loose coupling by integrating through events
- Outbox pattern to ensure at least once delivery
- Event stream analysis (even if over-simplified)
- Eventual consistency
- Idempotency, by ensuring the same event isn't handled multiple times