'use client'

import { useState, useRef, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'

const ReadMore = ({
  children,
  maxLines = 3,
  fadeHeight = 64,
  buttonClass = '',
  contentClass = '',
}) => {
  const [isExpanded, setIsExpanded] = useState(false)
  const [needsCollapse, setNeedsCollapse] = useState(false)
  const contentRef = useRef(null)

  useEffect(() => {
    if (contentRef.current) {
      const lineHeight = parseInt(getComputedStyle(contentRef.current).lineHeight)
      const maxHeight = lineHeight * maxLines
      setNeedsCollapse(contentRef.current.scrollHeight > maxHeight)
    }
  }, [maxLines])

  const toggleExpand = () => setIsExpanded(!isExpanded)

  return (
    <div className="relative">
      <motion.div
        className={`overflow-hidden ${contentClass}`}
        animate={{
          height: isExpanded ? 'auto' : `${maxLines * 1.5}em`,
        }}
        transition={{ duration: 0.3, ease: 'easeInOut' }}
      >
        <div ref={contentRef}>{children}</div>
        
        {/* Gradient overlay */}
        {!isExpanded && needsCollapse && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="absolute inset-x-0 bottom-0 h-16 pointer-events-none"
            style={{
              background: `linear-gradient(to bottom, transparent 0%, rgba(255,255,255,0.9) 70%, white 100%)`
            }}
          />
        )}
      </motion.div>

      {needsCollapse && (
        <div className="flex justify-center mt-4">
          <motion.button
            onClick={toggleExpand}
            className={`bg-white/90 backdrop-blur-sm px-6 py-2 rounded-full shadow-md 
              border border-gray-300 hover:bg-gray-50 transition-all ${buttonClass}`}
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
          >
            {isExpanded ? 'بستن' : 'بیشتر بخوانید'}
          </motion.button>
        </div>
      )}
    </div>
  )
}

export default ReadMore