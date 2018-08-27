Sprint.Linq [![NuGet](https://img.shields.io/nuget/v/Sprint.Linq.svg)](https://www.nuget.org/packages/Sprint.Linq/) [![Downloads](https://img.shields.io/nuget/dt/Sprint.Linq.svg)](https://www.nuget.org/packages/Sprint.Linq/)
===========

# Status
[![Build status](https://ci.appveyor.com/api/projects/status/wss9fr06lvathg79?svg=true)](https://ci.appveyor.com/project/artem-sedykh/sprint-linq)

# Examples

#### Simple expression mapper

```csharp
            Mapper.CreateMap<Company, CompanyView>()
                .DefaultMap(c => new CompanyView {Id = c.Id})
                .Include("name", x => new CompanyView {Name = x.Name, Id = x.Id})
                .Include("ordinal", x => new CompanyView {Ordinal = x.Ordinal});

            //entity => new CompanyView() {Id = entity.Id}
            var defaultMap = Mapper.Map<Company, CompanyView>();

            //entity => new CompanyView() {Ordinal = entity.Ordinal, Id = entity.Id}
            var ordinalMap = Mapper.Map<Company, CompanyView>("ordinal");

            //entity => new CompanyView() {Name = entity.Name, Id = entity.Id
            var defaultAndNameMap = Mapper.Map<Company, CompanyView>("name");

            //entity => new CompanyView() {Name = entity.Name, Id = entity.Id, Ordinal = entity.Ordinal}
            var defaultAndNameAndOrdinalMap = Mapper.Map<Company, CompanyView>("name", "ordinal");

            //entity => new CompanyView() {Name = entity.Name, Id = entity.Id, Ordinal = entity.Ordinal}
            var allMap = Mapper.MapAll<Company, CompanyView>();
```

### Expression extensions

```csharp
            Expression<Func<Company, bool>> left = l => l.Id == 15;

            Expression<Func<Company, bool>> right = r => r.Id == 20;

            //{l => ((l.Id == 15) OrElse (l.Id == 20))
            var disjunction = left.Or(right);

            //l => ((l.Id == 15) AndAlso (l.Id == 20))
            var conjunction = left.And(right);

            //l => Not((l.Id == 15)
            var negation = left.Not();
```

### Expression Helpers

```csharp
            Expression<Func<Interval, int>> begin = i => i.Start;
            Expression<Func<Interval, int>> end = i => i.End;

            //p => ((((p.Start >= 1) AndAlso (p.Start <= 10)) OrElse ((p.End >= 1) AndAlso (p.End <= 10))) OrElse ((p.Start <= 1) AndAlso (p.End >= 10)))
            var intersection = PredicateBuilder.Intersection(begin, end, 1, 10);
```

### Expression Expander

```csharp
            Expression<Func<Interval, bool>> testExpression = i => i.Start == 1;

            var paramenter = Expression.Parameter(typeof(Interval), "testParameter");

            //testParameter => Invoke(i => (i.Start == 1), testParameter)
            var expression = Expression.Lambda<Func<Interval, bool>>(Expression.Invoke(testExpression, paramenter), paramenter);

            //testParameter => (testParameter.Start == 1), xpanded expression
            var expandedExpression = expression.Expand();
```