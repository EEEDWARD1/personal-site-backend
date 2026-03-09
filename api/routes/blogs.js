const router = require('express').Router()
const blogController = require('../controllers/blogController')

router.get('/', blogController.getPublishedBlogs)
router.get('/:id', blogController.getById)

module.exports = router