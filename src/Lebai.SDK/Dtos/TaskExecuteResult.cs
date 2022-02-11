namespace Lebai.SDK.Dtos
{
   public class TaskExecuteResult
   {
      public int Id { get; set; }

      public TaskExecuteResult()
      {
      }

      public void Deconstruct(out int id)
      {
         id = Id;
      }
   }
}