using DataAccess.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using Utility;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PasswordManager _passwordManager;
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository, PasswordManager passwordManager)
        {
            _passwordManager = passwordManager;
            _userRepository = userRepository;
        }
        // GET: api/<UserController>
        [HttpGet]
        [Route("all")]
        public ActionResult Get()
        {
            IEnumerable<User> userList = _userRepository.GetAll();
            return Ok(userList);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = id;
            //condition["Email"] = EmailAddressAttribute;
            User? user = _userRepository.Get(condition);
            if (user == null)
            {
                return NotFound(ErrorResponse.NotFound());
            }
            return Ok(user);
        }

        // POST api/<UserController>
        [HttpPost]
        public ActionResult Post([FromBody] UserDto fuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            User user = new User
            {
                Name = fuser.Name,
                Email = fuser.Email,
                Password = _passwordManager.HashPassword(fuser.Password),
                CreatedAt = DateTime.Now,
                RoleId = 1,
                ModifiedAt = DateTime.Now,
            };
            int id = _userRepository.Add(user);

            if (id == 0)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Could not create user.", "User creation failed. Something catastrophically went wrong!"));
            }
            string uri = $"/api/user/{id}";
            user.Id = id;



            return Created(uri, user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = id;

            User? existingUser = _userRepository.Get(condition);
            if (existingUser == null)
            {
                return NotFound(ErrorResponse.ErrorCustom("Not Found", "No user was found"));
            }
            int modificationCounter = 0;
            if (userUpdateDto.Name != null)
            {
                modificationCounter++;
                existingUser.Name = userUpdateDto.Name;
            }
            if (userUpdateDto.Email != null)
            {
                modificationCounter++;
                existingUser.Email = userUpdateDto.Email;
            }


            if (modificationCounter > 0)
            {
                existingUser.ModifiedAt = DateTime.Now;
            }
            //check if password is valid for updating password


            if (userUpdateDto.NewPassword != null)
            {
                //making sure correct old password was supplied, otherwise fail completely
                if (userUpdateDto.Password == null || !_passwordManager.VerifyPassword(userUpdateDto.Password, existingUser.Password))
                {
                    //fail password check, return error
                    return StatusCode(StatusCodes.Status403Forbidden, ErrorResponse.ErrorCustom("Forbidden", "Password verification failed"));
                }
                //update password
                existingUser.Password = _passwordManager.HashPassword(userUpdateDto.NewPassword);
            }


            if (_userRepository.Update(existingUser) == null)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Entity not updated", "No property was updated."));
            }
            return Ok(existingUser);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            int status = _userRepository.Remove(id);

            if (status == 0)
            {
                return UnprocessableEntity(ErrorResponse.ErrorCustom("Failed to process id", "Nothing was deleted."));
            }
            return Ok(new
            {
                message = $"Successfully deleted {status} items."
            });
        }
    }
}
