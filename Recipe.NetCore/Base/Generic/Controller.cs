using Recipe.NetCore.Base.Abstract;
using Recipe.NetCore.Base.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Recipe.NetCore.Base.Generic
{
    [Authorize]
    [Route("")]
    public class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        //public async virtual Task<ResponseMessageResult> GetResponseMessage(HttpResponseMessage message)
        //{
        //    return this.ResponseMessage(message);
        //}
    }

    public abstract class Controller<TService, TDTO, TEntity, TKey> : Controller
     where TEntity : IAuditModel<TKey>, new()
     where TDTO : DTO<TEntity, TKey>, new()
     where TService : IService<TDTO, TKey>
    {
        TService _service;

        protected TService Service
        {
            get
            {
                return this._service;
            }
        }

        public Controller(TService service)
        {
            this._service = service;
        }

        [HttpGet]
        [Route("")]
        public virtual Task<IList<TDTO>> Get()
        {
            return this._service.GetAllAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TDTO> Get(TKey id)
        {
            return this._service.GetAsync(id);
        }

        [HttpPost]
        [Route("")]
        public virtual Task<TDTO> Post(TDTO dtoObject)
        {
            return this._service.CreateAsync(dtoObject);
        }

        [HttpPut]
        [Route("")]
        public virtual Task<TDTO> Put(TDTO dtoObject)
        {
            return this._service.UpdateAsync(dtoObject);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task Delete(TKey id)
        {
            return this._service.DeleteAsync(id);
        }
    }

}
