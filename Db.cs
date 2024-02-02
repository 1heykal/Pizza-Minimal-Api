namespace PizzaStore.DB;

public record Pizza{
    public int Id {get; set;}
    public string? Name {get; set;}
}

public class PizzaDB{
    private static List<Pizza> _Pizzas = new List<Pizza>(){
        new Pizza {Id = 1, Name = "Montemagno, Pizza shaped like a great mountain"},
        new Pizza {Id = 2, Name = "The Galloway, Pizza shaped like a submarine, silent but deadly"},
        new Pizza {Id = 3, Name = "The Noring, Pizza shaped like a Viking helmet, where's the mead"}
    };

    public static List<Pizza> GetPizzas(){
        return _Pizzas;
    }

    public static Pizza? GetPizza(int id){
        return _Pizzas.SingleOrDefault(pizza => pizza.Id == id);
    }

    public static Pizza CreatePizza(Pizza pizza){
        _Pizzas.Add(pizza);
        return pizza;
    }

    public static Pizza UpdatePizza(Pizza update){
      _Pizzas =  _Pizzas.Select(pizza => {
            if(pizza.Id == update.Id){
                pizza.Name = update.Name;
            }

            return pizza;
        }).ToList();

        return update;
    }

    public static void RemovePizza(int id){
        _Pizzas = _Pizzas.FindAll(pizza => pizza.Id != id).ToList();
    }
}