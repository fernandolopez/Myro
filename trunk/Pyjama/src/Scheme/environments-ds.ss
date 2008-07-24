;; Environments represented as data structures

;; continuations represented as data structures

;; bindings

(define make-binding
  (lambda (variable value)
    (cons variable value)))

(define binding-variable
  (lambda (binding)
    (car binding)))

(define binding-value
  (lambda (binding)
    (cdr binding)))

(define set-binding-value!
  (lambda (binding value)
    (set-cdr! binding value)))

;; frames

(define make-frame
  (lambda (variables values)
    (map make-binding variables values)))

(define first-binding
  (lambda (frame)
    (car frame)))

(define rest-of-bindings
  (lambda (frame)
    (cdr frame)))

(define empty-frame?
  (lambda (frame)
    (null? frame)))

(define search-frame
  (lambda (frame variable)
    (cond
      ((empty-frame? frame) #f)
      ((eq? (binding-variable (first-binding frame)) variable)
       (first-binding frame))
      (else (search-frame (rest-of-bindings frame) variable)))))

;; environments

(define make-empty-environment
  (lambda ()
    '(())))

(define make-initial-environment
  (lambda (vars vals)
    (list (make-frame vars vals))))

(define first-frame
  (lambda (env)
    (car env)))

(define rest-of-frames
  (lambda (env)
    (cdr env)))

(define set-first-frame!
  (lambda (env new-frame)
    (set-car! env new-frame)))

(define extend
  (lambda (env variables values)
    (cons (make-frame variables values) env)))

;; variable lookup

(define search-env
  (lambda (env variable)
    (if (null? env)
      #f
      (let ((binding (search-frame (first-frame env) variable)))
        (if binding
          binding
          (search-env (rest-of-frames env) variable))))))

(define lookup-binding
  (lambda (variable env handler k)
    (let ((binding (search-env env variable)))
      (if binding
	(apply-cont k binding)
	(apply-handler handler (format "unbound variable ~s" variable))))))

(define lookup-value
  (lambda (variable env handler k)
    (lookup-binding variable env handler
      (make-cont 'parser 'lookup-cont k))))

;; adds a new binding for var to the first frame if one doesn't exist
(define lookup-binding-in-first-frame
  (lambda (var env handler k)
    (let ((frame (first-frame env)))
      (let ((binding (search-frame frame var)))
        (if binding
	  (apply-cont k binding)
          (let ((new-binding (make-binding var 'undefined)))
            (let ((new-frame (cons new-binding frame)))
              (set-first-frame! env new-frame)
	      (apply-cont k new-binding))))))))