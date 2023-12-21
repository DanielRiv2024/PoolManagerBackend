using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IEvalucionInstructor
    {
        public Task<List<EvaluacionInstructor>> GetAll();
        public Task<Boolean> Create(EvaluacionInstructor evaluacionInstructor);
        public Task<Boolean> Delete(int id);    
        public Task<EvaluacionInstructor>  Edit(EvaluacionInstructor evaluacionInstructor);
        public Task<EvaluacionInstructor> GetEvaluacionInstructorById(int id);
    }
}
