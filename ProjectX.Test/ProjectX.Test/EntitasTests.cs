using Entitas;
using NSpec;

class EntitasTests : nspec
{
    protected Contexts Contexts;
    protected Systems Systems;

    protected void Setup()
    {
        Contexts = new Contexts();
        Systems = new Systems();
    }
}