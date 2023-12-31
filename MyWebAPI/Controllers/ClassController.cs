﻿using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using X.PagedList;

namespace MyWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize]
    public class ClassController : ControllerBase
	{
		private readonly IClassRepository _classRepository;
		private readonly IMapper _mapper;

		public ClassController(IClassRepository classRepository, IMapper mapper)
		{
			_classRepository = classRepository;
			_mapper = mapper;
		}
        public enum FilterType
        {
            ClassName
        }

        /// <summary>
        /// Show list of Class
        /// </summary>
        /// <returns></returns>
        /// <param name="filterValue">Input the Value you want to filt. Filt by ClassName.</param>
        /// <param name="page">Index starting from 0 to designate the page for retrieval.</param>
		/// <param name="pageSize">Number of results per page to return</param>
        /// <remarks>
        /// Returns the lists of **Class** that have been assigned access control on the referenced resource.
        /// </remarks>
        /// <response code="200">Successfully returns a list of Class.</response>
        [HttpGet]
		[Authorize(Roles = "Reader")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ClassDTO>))]
		public IActionResult GetAllClasses(string filterValue = "",int? page = 0, int pageSize = 10)
		{
            IEnumerable<DataAccessLayer.Models.Classes> classes = _classRepository.GetClasses();

            if (!string.IsNullOrEmpty(filterValue))
            {
                classes = classes.Where(t => t.ClassName.Contains(filterValue)).ToList();
            }

            var pagedClasses = classes.ToPagedList(page ?? 0, pageSize);
            var classMap = _mapper.Map<IEnumerable<ClassDTO>>(classes);

            return Ok(classMap);
        }

        /// <summary>
        /// Get a Class by ClassId
        /// </summary>
        /// <param name="id">Input ClassId to see **Class's info**</param>
        /// <returns></returns>
        /// <remarks>
        /// Retrieve *class* by **ClassId** and can custom select *any class*.
        /// 
        /// **Note**: If you need to find a Class but do not know the Class's Name,
        /// you can search on the System through the **ClassId**.
        /// </remarks>
        /// <response code="200">Information of Class</response>
        /// <response code="404">Class Id Not Found!!</response>
        [HttpGet("{id}")]
		[Authorize(Roles = "Reader")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClassDTO))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetClassById(int id)
		{
			var classEntity = _classRepository.GetClass(id);

			if (classEntity == null)
				return NotFound();

			var classMap = _mapper.Map<ClassDTO>(classEntity);

			return Ok(classMap);
		}

        /// <summary>
        /// Create a Class
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Create a Class by input **Class Name**.
        /// </remarks>
        /// <response code="201">Successfully created a Class.</response>
        /// <response code="400">Class domain is not among the registered SSO 
        /// domains for this System!!</response>
        [HttpPost]
		[Authorize(Roles = "Writer")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClassDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult CreateClass([FromBody] ClassDTO classDTO)
		{
			var classEntity = _mapper.Map<Classes>(classDTO);

			_classRepository.AddClass(classEntity);

			classDTO.Id = classEntity.Id;


			return CreatedAtAction(nameof(GetClassById), new { id = classDTO.Id }, classDTO);
		}

        /// <summary>
        /// Update a Class by ClassId
        /// </summary>
        /// <param name="id">Input ClassId to **update** Class's info.</param>
        /// <param name="classDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Update the specified _class_ to the organization by **Class Id**.
        /// </remarks>
        /// <response code="204">Class's Info updated successfully</response>
        /// <response code="400">ClassId domain is not among the registered SSO 
        /// domains for this System!!</response>
        /// <response code="404">ClassId Not Found!!</response>
        [HttpPut("{id}")]
		[Authorize(Roles = "Editor")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateClass(int id, [FromBody] ClassDTO classDTO)
		{
			var existingClass = _classRepository.GetClass(id);

			if (existingClass == null)
			{
				return NotFound();
			}

			_classRepository.UpdateClass(id, classDTO);

			return NoContent();
		}

        /// <summary>
        /// Delete a Class's Info by ClassId
        /// </summary>
        /// <param name="id">Input ClassId to **delete** Class's info.</param>
        /// <returns></returns>
        /// <remarks>
        /// **Note:** Removes the specified Class from the list by **ClassId**.
        /// 
        /// </remarks>
        /// <response code="204">Class's Info deleted successfully</response>
        /// <response code="404">ClassId Not Found!!</response>
        [HttpDelete("{id}")]
		[Authorize(Roles = "Editor")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteClass(int id)
		{
			var existingClass = _classRepository.GetClass(id);

			if (existingClass == null)
				return NotFound();

			_classRepository.DeleteClass(existingClass);

			return NoContent();
		}
	}
}
