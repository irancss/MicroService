# Search Service Architecture

## System Architecture Flow

```mermaid
graph TB
    %% External clients
    Client[Web/Mobile Client]
    Admin[Admin Dashboard]
    
    %% Load balancer
    NGINX[NGINX Load Balancer<br/>Rate Limiting & Caching]
    
    %% API instances
    API1[Search API Instance 1]
    API2[Search API Instance 2]
    API3[Search API Instance 3]
    
    %% Application layers
    subgraph "API Layer"
        Controllers[Controllers<br/>Search, Index, Health]
        Middleware[Middleware<br/>Validation, Logging, CORS]
    end
    
    subgraph "Application Layer (CQRS)"
        Handlers[MediatR Handlers]
        Commands[Commands<br/>IndexProduct, RemoveProduct]
        Queries[Queries<br/>ProductSearch, Suggestions]
        Behaviors[Pipeline Behaviors<br/>Validation, Logging, Performance]
    end
    
    subgraph "Domain Layer"
        Entities[Entities<br/>ProductDocument, UserPersonalization]
        ValueObjects[Value Objects<br/>SearchFacet, PriceRange]
        Interfaces[Interfaces<br/>ISearchRepository, IPersonalizationService]
    end
    
    subgraph "Infrastructure Layer"
        ESRepo[Elasticsearch Repository]
        QueryService[Elasticsearch Query Service]
        PersonalizationService[User Personalization Service]
        MessageConsumers[MassTransit Consumers]
    end
    
    %% External services
    Elasticsearch[(Elasticsearch Cluster<br/>Product Search Index)]
    RabbitMQ[(RabbitMQ<br/>Message Broker)]
    PersonalizationAPI[User Personalization API]
    ProductService[Product Service]
    
    %% Monitoring
    Kibana[Kibana<br/>Search Analytics]
    Prometheus[Prometheus<br/>Metrics Collection]
    Grafana[Grafana<br/>Monitoring Dashboards]
    
    %% Client connections
    Client --> NGINX
    Admin --> NGINX
    
    %% Load balancing
    NGINX --> API1
    NGINX --> API2
    NGINX --> API3
    
    %% API flow
    API1 --> Controllers
    API2 --> Controllers
    API3 --> Controllers
    Controllers --> Middleware
    Middleware --> Handlers
    
    %% CQRS flow
    Handlers --> Commands
    Handlers --> Queries
    Commands --> Behaviors
    Queries --> Behaviors
    Behaviors --> Entities
    Behaviors --> ValueObjects
    Behaviors --> Interfaces
    
    %% Infrastructure connections
    Interfaces --> ESRepo
    Interfaces --> QueryService
    Interfaces --> PersonalizationService
    ESRepo --> Elasticsearch
    QueryService --> Elasticsearch
    PersonalizationService --> PersonalizationAPI
    
    %% Message flow
    ProductService -->|Product Events| RabbitMQ
    RabbitMQ --> MessageConsumers
    MessageConsumers --> Commands
    
    %% Monitoring connections
    Elasticsearch --> Kibana
    API1 --> Prometheus
    API2 --> Prometheus
    API3 --> Prometheus
    Prometheus --> Grafana
    
    %% Styling
    classDef client fill:#e1f5fe
    classDef api fill:#f3e5f5
    classDef data fill:#e8f5e8
    classDef monitor fill:#fff3e0
    classDef message fill:#fce4ec
    
    class Client,Admin client
    class NGINX,API1,API2,API3,Controllers,Middleware,Handlers api
    class Elasticsearch,RabbitMQ,PersonalizationAPI data
    class Kibana,Prometheus,Grafana monitor
    class MessageConsumers,ProductService message
```

## Data Flow Diagram

```mermaid
sequenceDiagram
    participant Client
    participant NGINX
    participant SearchAPI
    participant MediatR
    participant ESQuery
    participant Elasticsearch
    participant PersonalizationAPI
    
    %% Search Request Flow
    Client->>NGINX: GET /api/search/products?query=laptop&userId=123
    Note over NGINX: Rate limiting<br/>Caching check
    NGINX->>SearchAPI: Forward request
    SearchAPI->>MediatR: ProductSearchQuery
    
    %% Personalization
    MediatR->>PersonalizationAPI: GET /users/123/personalization
    PersonalizationAPI-->>MediatR: User preferences
    
    %% Search execution
    MediatR->>ESQuery: Build complex query with personalization
    ESQuery->>Elasticsearch: Execute search with boosts & facets
    Elasticsearch-->>ESQuery: Results + aggregations
    ESQuery-->>MediatR: Mapped response
    MediatR-->>SearchAPI: ProductSearchResponse
    
    %% Response caching
    SearchAPI-->>NGINX: Search results
    Note over NGINX: Cache response<br/>1 minute TTL
    NGINX-->>Client: JSON response with products & facets
```

## Event-Driven Index Updates

```mermaid
sequenceDiagram
    participant ProductService
    participant RabbitMQ
    participant SearchConsumer
    participant MediatR
    participant ESRepository
    participant Elasticsearch
    
    %% Product creation flow
    ProductService->>RabbitMQ: Publish ProductCreatedEvent
    RabbitMQ->>SearchConsumer: Consume event
    SearchConsumer->>MediatR: IndexProductCommand
    MediatR->>ESRepository: Index product document
    ESRepository->>Elasticsearch: Index API call
    Elasticsearch-->>ESRepository: Success response
    ESRepository-->>MediatR: Indexing result
    MediatR-->>SearchConsumer: Command response
    
    Note over SearchConsumer: Product now searchable
    
    %% Product update flow
    ProductService->>RabbitMQ: Publish ProductUpdatedEvent
    RabbitMQ->>SearchConsumer: Consume event
    SearchConsumer->>MediatR: IndexProductCommand (upsert)
    MediatR->>ESRepository: Update product document
    ESRepository->>Elasticsearch: Update API call
    
    %% Product deletion flow
    ProductService->>RabbitMQ: Publish ProductDeletedEvent
    RabbitMQ->>SearchConsumer: Consume event
    SearchConsumer->>MediatR: RemoveProductFromIndexCommand
    MediatR->>ESRepository: Delete product document
    ESRepository->>Elasticsearch: Delete API call
```

## Elasticsearch Index Structure

```mermaid
graph TB
    subgraph "Products Index"
        subgraph "Document Fields"
            Id[id: keyword]
            Name[name: text + keyword + completion]
            Description[description: text]
            Category[category: keyword + text]
            Brand[brand: keyword + text]
            Price[price: scaled_float]
            Rating[averageRating: half_float]
            Available[isAvailable: boolean]
            Tags[tags: keyword + text]
            Attributes[attributes: object<br/>dynamic mapping]
        end
        
        subgraph "Analysis Chain"
            ProductAnalyzer[product_analyzer<br/>standard + lowercase + stop + stemmer]
            AutocompleteAnalyzer[autocomplete_analyzer<br/>keyword + lowercase]
            SearchAnalyzer[search_analyzer<br/>standard + lowercase + stop + stemmer]
        end
        
        subgraph "Search Features"
            FullText[Full-text Search<br/>Multi-match across fields]
            Faceting[Dynamic Faceting<br/>Terms & Range aggregations]
            Suggestions[Auto-complete<br/>Completion suggester]
            Personalization[Personalized Boosting<br/>Function score queries]
        end
        
        subgraph "Performance Optimizations"
            Sharding[3 Primary Shards<br/>1 Replica each]
            Caching[Query result caching<br/>Fielddata caching]
            Routing[Custom routing<br/>by category]
            Refresh[Near real-time<br/>1 second refresh]
        end
    end
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Production Environment"
        subgraph "Load Balancer Tier"
            LB1[NGINX Instance 1]
            LB2[NGINX Instance 2]
        end
        
        subgraph "Application Tier"
            subgraph "Search API Pods"
                Pod1[Search API Pod 1<br/>2 CPU, 1GB RAM]
                Pod2[Search API Pod 2<br/>2 CPU, 1GB RAM]
                Pod3[Search API Pod 3<br/>2 CPU, 1GB RAM]
                PodN[Search API Pod N<br/>Auto-scaled]
            end
        end
        
        subgraph "Data Tier"
            subgraph "Elasticsearch Cluster"
                ES1[ES Master Node 1<br/>4 CPU, 8GB RAM]
                ES2[ES Data Node 1<br/>8 CPU, 16GB RAM]
                ES3[ES Data Node 2<br/>8 CPU, 16GB RAM]
                ES4[ES Data Node 3<br/>8 CPU, 16GB RAM]
            end
            
            subgraph "Message Queue Cluster"
                RMQ1[RabbitMQ Node 1]
                RMQ2[RabbitMQ Node 2]
                RMQ3[RabbitMQ Node 3]
            end
        end
        
        subgraph "Monitoring Tier"
            Prometheus[Prometheus<br/>Metrics Collection]
            Grafana[Grafana<br/>Dashboards]
            Kibana[Kibana<br/>Log Analysis]
            AlertManager[AlertManager<br/>Alerting]
        end
        
        subgraph "Storage"
            ESStorage[(Elasticsearch Storage<br/>SSD, 1TB per node)]
            RMQStorage[(RabbitMQ Storage<br/>SSD, 100GB)]
            LogStorage[(Log Storage<br/>ELK Stack)]
        end
    end
    
    %% External connections
    Internet[Internet] --> LB1
    Internet --> LB2
    
    LB1 --> Pod1
    LB1 --> Pod2
    LB2 --> Pod2
    LB2 --> Pod3
    LB2 --> PodN
    
    Pod1 --> ES2
    Pod2 --> ES3
    Pod3 --> ES4
    PodN --> ES2
    
    Pod1 --> RMQ1
    Pod2 --> RMQ2
    Pod3 --> RMQ3
    
    ES1 --> ESStorage
    ES2 --> ESStorage
    ES3 --> ESStorage
    ES4 --> ESStorage
    
    RMQ1 --> RMQStorage
    RMQ2 --> RMQStorage
    RMQ3 --> RMQStorage
    
    Pod1 --> Prometheus
    Pod2 --> Prometheus
    Pod3 --> Prometheus
    
    Prometheus --> Grafana
    ES1 --> Kibana
    Prometheus --> AlertManager
```

This architectural documentation provides:

1. **System Overview**: Complete flow from client request to response
2. **Data Flow**: Detailed sequence of search operations
3. **Event Processing**: How real-time updates work
4. **Index Structure**: Elasticsearch configuration and optimization
5. **Deployment**: Production-ready architecture with scaling considerations

The diagrams show how the Clean Architecture principles are maintained while achieving high performance and scalability through proper separation of concerns and infrastructure optimization.
