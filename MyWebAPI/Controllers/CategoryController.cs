﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Collections.Generic;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Repositories;


namespace MyWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
		}
        public enum FilterType
        {
            CategoriesName
        }

        /// <summary>
        /// Show list of Category
        /// </summary>
        /// <returns></returns>
        /// <param name="filterValue">Input the Value you want to filt. Filt by CategoryName.</param>
        /// <param name="page">Index starting from 0 to designate the page for retrieval.</param>
		/// <param name="pageSize">Number of results per page to return.</param>
        /// <remarks>
        /// Returns the lists of **Category** that have been assigned access control on the referenced resource.
        /// </remarks>
        /// <response code="200">Successfully returns a list of Category.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDTO>))]
        [Authorize(Roles = "Reader")]
		public IActionResult GetAllCategories(string filterValue = "",int? page = 0, int pageSize = 10)
		{
            IEnumerable<DataAccessLayer.Models.Categories> categories = _categoryRepository.GetCategories();

            if (!string.IsNullOrEmpty(filterValue))
            {
                categories = categories.Where(t => t.CategoriesName.Contains(filterValue)).ToList();
            }

            var pagedCatagories = categories.ToPagedList(page ?? 0, pageSize);
            var categoryMap = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

			return Ok(categoryMap);
		}

        /// <summary>
        /// Get a Category by CategoryId
        /// </summary>
        /// <param name="id">Input Category's Id to see **Category's info**</param>
        /// <returns></returns>
        /// <remarks>
        /// Retrieve *categories* by **Category Id** and can custom select *any category*.
        /// 
        /// **Note**: If you need to find a Category but do not know the Category's Name,
        /// you can search on the System through the **Category Id**
        /// </remarks>
        /// <response code="200">Information of Category</response>
        /// <response code="404">Category Id Not Found!!</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Reader")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult GetCategoryById(int id)
		{
			var category = _categoryRepository.GetCategory(id);

			if (category == null)
				return NotFound();

			var categoryMap = _mapper.Map<CategoryDTO>(category);

			return Ok(categoryMap);
		}

        /// <summary>
        /// Create a Category
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Create a Category by input **Category Name**.
        /// </remarks>
        /// <response code="201">Successfully created a Category.</response>
		/// <response code="400">Category domain is not among the registered SSO 
		/// domains for this System!!</response>
        [HttpPost]
        [Authorize(Roles = "Writer")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoryDTO))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult CreateCategory([FromBody] CategoryDTO categoryDTO)
		{
			var categoryEntity = _mapper.Map<Categories>(categoryDTO);

			_categoryRepository.AddCategory(categoryEntity);

			categoryDTO.Id = categoryEntity.Id;

			return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDTO.Id }, categoryDTO);
		}

        /// <summary>
        /// Update a Category by CategoryId
        /// </summary>
        /// <param name="id">Input CategoryId to **update** Category's info.</param>
        /// <param name="categoryDTO"></param>
        /// <returns></returns>
        /// <remarks>
        /// Update the specified _category_ to the System by **Category Id**.
        /// </remarks>
        /// <response code="204">Category's Info updated successfully</response>
        /// <response code="400">CategoryId domain is not among the registered SSO 
        /// domains for this System!!</response>
        /// <response code="404">CategoryId Not Found!!</response>
        [HttpPut("{id}")]
		[Authorize(Roles = "Editor")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
		{
			var existingCategory = _categoryRepository.GetCategory(id);

			if (existingCategory == null)
				return NotFound();

			_categoryRepository.UpdateCategory(id, categoryDTO);

			return NoContent();
		}

        /// <summary>
        /// Delete a Category's Info by CategoryId
        /// </summary>
        /// <param name="id">Input CategoryId to **delete** Category's info.</param>
        /// <returns></returns>
        /// <remarks>
        /// **Note:** Removes the specified Category from the líst by **CategoryId**.
        /// 
        /// </remarks>
        /// <response code="204">Category's Info deleted successfully</response>
        /// <response code="404">CategoryId Not Found!!</response>
        [HttpDelete("{id}")]
		[Authorize(Roles = "Editor")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteCategory(int id)
		{
			var existingCategory = _categoryRepository.GetCategory(id);

			if (existingCategory == null)
				return NotFound();

			_categoryRepository.DeleteCategory(existingCategory);

			return NoContent();
		}
	}
}
