using ApplicationLayer.CQRS.DTOs;
using DomainLayer.EntityModels;

namespace ApplicationLayer.CQRS.Converters
{
    public static class CustomeConverters
    {
        public static AccountDTO ToDTO(this Account account)
        {
            var accountDto = new AccountDTO()
            {
                Id = account.Id,
                Active = account.Active,
                CreatedBy = account.CreatedBy,
                CreatedDate = account.CreatedDate,
                UpdateDate = account.UpdateDate,
                UpdateBy = account.UpdateBy,

                UserName = account.UserName,
                CustomerId = account.CustomerId
            };

            return accountDto;
        }

        public static CustomerDTO ToDTO(this Customer customer)
        {
            var customerDTO = new CustomerDTO()
            {
                Id = customer.Id,
                Active = customer.Active,
                CreatedBy = customer.CreatedBy,
                CreatedDate = customer.CreatedDate,
                UpdateBy = customer.UpdateBy,
                UpdateDate = customer.UpdateDate,

                Name = customer.Name,
                Surname = customer.Surname,
                Email = customer.Email,
                Balance = customer.Balance,
                AccountId = customer.AccountId
            };

            return customerDTO;
        }

        public static TransactionDTO ToDTO(this Transaction transaction)
        {
            var transactionDTO = new TransactionDTO()
            {
                Id = transaction.Id,
                Active = transaction.Active,
                CreatedBy = transaction.CreatedBy,
                CreatedDate = transaction.CreatedDate,
                UpdateDate = transaction.UpdateDate,
                UpdateBy = transaction.UpdateBy,

                SenderId = transaction.SenderId,
                RecipientId = transaction.RecipientId,
                TransferredAmount = transaction.TransferredAmount
            };

            return transactionDTO;
        }


    }
}
