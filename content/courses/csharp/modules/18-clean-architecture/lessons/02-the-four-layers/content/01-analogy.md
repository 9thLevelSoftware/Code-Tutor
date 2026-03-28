---
type: "ANALOGY"
title: "The Layer Cake Model"
---

Think of Clean Architecture as baking a magnificent four-layer cake - each layer has its own unique purpose, flavor, and ingredients, but together they create something far greater than the sum of their parts.

THE FOUR-LAYER CAKE:

**THE BOTTOM LAYER - DOMAIN (Rich Chocolate Foundation)**
This is your dense, rich chocolate cake layer at the bottom. It's the foundation that holds everything up. You bake this layer first, and it needs to be absolutely perfect because everything else rests on it. In software, the Domain layer contains your business entities, value objects, and core business rules. Just like how a chocolate cake layer doesn't need frosting or decorations to be chocolate - it IS chocolate - your Domain layer doesn't need databases or HTTP to define what a Product or Order is. It simply IS your business logic.

**THE SECOND LAYER - APPLICATION (Vanilla Coordination Layer)**
This is your vanilla layer that sits on top of the chocolate. It's lighter, it coordinates flavors, and it decides how the chocolate layer below will be experienced. The Application layer contains your use cases - the recipes for what your application can do. It defines interfaces (like saying 'I need frosting here') without actually providing the frosting. CreateOrderUseCase, ProcessPaymentUseCase - these are your recipes that orchestrate the domain below.

**THE THIRD LAYER - INFRASTRUCTURE (The Frosting and Filling)**
The frosting and filling are what connect everything together and make the cake deliverable. You can swap buttercream for cream cheese frosting without changing the cake layers themselves. Infrastructure is your Entity Framework DbContext, your email services, your payment gateways. It implements the interfaces defined by Application. Want to switch from SQL Server to PostgreSQL? Just change the frosting - the cake layers remain untouched.

**THE TOP LAYER - PRESENTATION (The Decorations)**
The decorations on top - the fondant roses, sprinkles, and 'Happy Birthday' writing. This is what the customer sees and interacts with. It can be completely redesigned without touching the cake itself. Your API controllers, Blazor components, and console interfaces live here. You could have the same cake with birthday decorations, wedding decorations, or minimalist modern decorations.

**THE CRITICAL RULE:**
When you slice the cake, each layer only touches the layer directly below it. The decorations sit on the frosting, not embedded in the chocolate. The frosting sits on the vanilla, not mixed into it. Dependencies flow DOWN through the layers, never up. The chocolate layer has no idea what decorations are on top - it just focuses on being the best chocolate layer it can be.

Think: 'Each layer depends only on the layer directly below it. The Domain cake layer has zero knowledge of what decorations might someday sit on top!'