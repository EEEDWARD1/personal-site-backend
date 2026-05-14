const router = require('express').Router()
const auth = require('../middleware/auth')
const blogController = require('../controllers/blogController')

router.use(auth) // applies to all routes below

router.get('/blogs', blogController.getAllBlogs)
router.post('/blogs', blogController.createBlog)
router.put('/blogs/:id', blogController.updateBlog)
router.delete('/blogs/:id', blogController.deleteBlog)

module.exports = router