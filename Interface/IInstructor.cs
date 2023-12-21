using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IInstructor
    {
        public Task<List<Instructor>> GetAll();
        public Task<Boolean> Create(Instructor instructor);
        public Task<Boolean> Delete(int id);
        public Task<EvaluacionInstructor> Edit(Instructor instructor);
    }
}
