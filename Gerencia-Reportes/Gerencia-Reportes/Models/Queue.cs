
namespace Gerencia_Reportes.Models
{
    public class Queue
    {
        public string QueueID { get; set; }
        public string Name { get; set; }

        public bool IsSelected { get; set; } = false;

        public Queue()
        {

        }
        public class QueueBuilder
        {
            private Queue queue;

            public QueueBuilder()
            {
                queue = new Queue();
            }

            public QueueBuilder WithQueueID(string id)
            {
                queue.QueueID = id;
                return this;
            }

            public QueueBuilder WithName(string name)
            {
                queue.Name = name;
                return this;
            }
            public QueueBuilder WithIsSelected(bool isSelected)
            {
                queue.IsSelected = isSelected;
                return this;
            }


            public Queue Build()
            {
                return queue;
            }
        }
    }
}
