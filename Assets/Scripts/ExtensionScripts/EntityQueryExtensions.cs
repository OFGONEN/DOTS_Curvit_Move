using Unity.Entities;

namespace Curvit.Demos.DOTS_Move
{
    public static class EntityQueryExtensions
    {
        public static T GetClassSingleton<T>(this EntityManager entityManager)
        {
            var entity = entityManager
                .CreateEntityQuery(typeof(T))
                .GetSingletonEntity();

            return entityManager.GetComponentObject<T>(entity);
        }
    }
}
