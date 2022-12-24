using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_dataContext, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_dataContext, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_dataContext);

        public async Task<bool> CompleteAsync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public bool HasChange()
        {
            return _dataContext.ChangeTracker.HasChanges();
        }
    }
}